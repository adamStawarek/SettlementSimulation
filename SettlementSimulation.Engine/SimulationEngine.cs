﻿using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine
{
    public class SimulationEngine
    {

        #region properties
        public Epoch CurrentEpoch { get; set; }
        public int Generation { get; set; }
        public Field[,] Fields { get; }
        public List<Point> MainRoad { get; }
        public SettlementUpdate LastSettlementUpdate { get; set; }
        public Settlement Settlement { get; set; }
        #endregion

        public SimulationEngine(
            Field[,] fields,
            List<Point> mainRoad)
        {
            Fields = fields;
            MainRoad = mainRoad;
            Generation = 1;
            Settlement = new Settlement(fields, mainRoad);
            CurrentEpoch = Epoch.First;
        }

        public void NewGeneration()
        {
            var updateType = GetUpdateType();

            var structures = Enumerable.Range(1, 100).ToList()
                        .Select(s => Settlement.CreateNewSettlementUpdate(updateType, CurrentEpoch))
                        .ToList();

            var settlementUpdate = GetBestStructures(structures);

            LastSettlementUpdate = settlementUpdate;

            switch (updateType)
            {
                case UpdateType.NewRoads:
                    {
                        var roadTypeSetUp = new RoadTypeSetUp()
                        {
                            Epoch = CurrentEpoch,
                            SettlementCenter = Settlement.SettlementCenter,
                            AvgDistanceToSettlementCenter =
                                (int)Settlement.Genes.Average(r => r.Center.DistanceTo(Settlement.SettlementCenter))
                        };
                        settlementUpdate.NewRoads
                            .ForEach(r =>
                            {
                                r.SetUpRoadType(roadTypeSetUp);
                                Settlement.AddRoad(r);
                            });
                        break;
                    }
                case UpdateType.NewBuildings:
                    settlementUpdate.NewBuildings
                        .ForEach(b => { Settlement.AddBuildingToRoad(b.Road, b); });
                    break;
                case UpdateType.NewTypes:
                    settlementUpdate.UpdatedBuildings.ForEach(update =>
                    {
                        Settlement.RemoveBuildingFromRoad(update.oldBuilding.Road, update.oldBuilding);
                        Settlement.AddBuildingToRoad(update.newBuilding.Road, update.newBuilding);
                    });
                    settlementUpdate.UpdatedRoads.ForEach(update =>
                    {
                        update.oldRoad.SetRoadType(update.newRoad.Type); //todo for now only types
                    });
                    break;
            }

            this.Settlement.Buildings.ForEach(b => b.Age++);

            if (RandomProvider.NextDouble() < 0.001)//todo
            {
                settlementUpdate.FloodMutationResult = Settlement.InvokeFloodMutation();
            }

            if (RandomProvider.NextDouble() < 0.01)//todo
            {
                settlementUpdate.EarthquakeMutationResult = Settlement.InvokeEarthquakeMutation();
            }

            if (EpochSpecific.CanEnterNextEpoch(Settlement, CurrentEpoch))
            {
                switch (CurrentEpoch)
                {
                    case Epoch.First:
                        CurrentEpoch = Epoch.Second;
                        break;
                    case Epoch.Second:
                        CurrentEpoch = Epoch.Third;
                        break;
                    case Epoch.Third:
                        //TODO
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Generation++;
        }

        private UpdateType GetUpdateType()
        {
            var config = ConfigurationManager.SettlementConfiguration[CurrentEpoch];
            UpdateType updateType;
            switch (RandomProvider.NextDouble())
            {
                case double d when d < config.ProbNewRoad:
                    {
                        updateType = UpdateType.NewRoads;
                        break;
                    }
                case double d when d < config.ProbNewRoad + config.ProbNewBuildings:
                    {
                        updateType = UpdateType.NewBuildings;
                        break;
                    }
                default:
                    updateType = UpdateType.NewTypes;
                    break;
            }

            return updateType;
        }

        private SettlementUpdate GetBestStructures(List<SettlementUpdate> updates)
        {
            var structuresFitness = updates.ToDictionary(s => s, s => 0);
            foreach (var key in updates)
            {
                var fitness = CalculateFitness(key);
                structuresFitness[key] = fitness;
            }

            var bests = structuresFitness.OrderByDescending(s => s.Value).Take(2).Select(u => u.Key).ToArray();
            var settlementUpdate = bests[0].Crossover(bests[1]);

            return settlementUpdate;
        }

        private int CalculateFitness(SettlementUpdate model)
        {
            var fitness = 0;

            switch (model.UpdateType)
            {
                case UpdateType.NewRoads:
                    foreach (var road in model.NewRoads)
                    {
                        fitness += 1;
                        var roads = new List<IRoad>(Settlement.Genes) { road };
                        road.Buildings.ForEach(b => b.SetFitness(new BuildingRule()
                        {
                            Fields = Settlement.Fields,
                            Roads = roads,
                            BuildingRoad = road,
                            SettlementCenter = Settlement.SettlementCenter
                        }));
                        fitness += road.Buildings.Sum(b => b.Fitness);
                    }
                    break;
                case UpdateType.NewBuildings:
                    foreach (var building in model.NewBuildings)
                    {
                        var roads = new List<IRoad>(Settlement.Genes);
                        building.SetFitness(new BuildingRule()
                        {
                            Fields = Settlement.Fields,
                            Roads = roads,
                            BuildingRoad = building.Road,
                            SettlementCenter = Settlement.SettlementCenter
                        });
                        fitness += building.Fitness;
                    }
                    break;
                case UpdateType.NewTypes:
                    foreach (var (oldBuilding, newBuilding) in model.UpdatedBuildings)
                    {
                        var roads = new List<IRoad>(Settlement.Genes);
                        newBuilding.SetFitness(new BuildingRule()
                        {
                            Fields = Settlement.Fields,
                            Roads = roads,
                            BuildingRoad = newBuilding.Road,
                            SettlementCenter = Settlement.SettlementCenter
                        });
                        fitness += newBuilding.Fitness - oldBuilding.Fitness;
                    }
                    break;
            }

            return fitness;
        }
    }
}