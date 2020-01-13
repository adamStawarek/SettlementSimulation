using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine
{
    public class SimulationEngine
    {
        #region private fields
        private readonly Stack<Epoch> _allEpochs;
        #endregion

        #region properties
        public Epoch CurrentEpoch { get; set; }
        public int Generation { get; set; }
        public Field[,] Fields { get; }
        public List<Point> MainRoad { get; }
        public IEnumerable<ISettlementStructure> LastStructuresCreated { get; set; }
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

            _allEpochs = new Stack<Epoch>();
            _allEpochs.Push(Epoch.Third);
            _allEpochs.Push(Epoch.Second);
            _allEpochs.Push(Epoch.First);
            SetNextEpoch();
        }

        public void SetNextEpoch()
        {
            CurrentEpoch = _allEpochs.Pop();
        }

        public void NewGeneration()
        {
            var structures = Enumerable.Range(1, 5).ToList()
                .Select(s => Settlement.CreateNewDnaStructure(CurrentEpoch))
                .ToList();

            var generatedStructures = GetBestStructures(structures);

            LastStructuresCreated = null;
            if (generatedStructures.NewRoads.Any())
            {
                generatedStructures.NewRoads
                    .ForEach(r => Settlement.AddRoad(r));
                LastStructuresCreated = generatedStructures.NewRoads.ToList();
            }

            if (generatedStructures.NewBuildings.Any())
            {
                generatedStructures.NewBuildings
                    .ForEach(b =>
                    {
                        Settlement.AddBuildingToRoad(generatedStructures.RoadToAttachNewBuildings, b);
                        LastStructuresCreated = generatedStructures.NewBuildings.ToList();
                    });
            }

            if (Settlement.Buildings.Count >= EpochSpecific.GetBuildingsCount(CurrentEpoch))
            {
                SetNextEpoch();
            }

            Generation++;
        }

        private GeneratedStructures GetBestStructures(List<GeneratedStructures> structures)
        {
            var structuresFitness = structures.ToDictionary(s => s, s => 0);
            foreach (var structure in structures)
            {
                var fitness = CalculateGeneratedStructuresFitness(structure);
                structuresFitness[structure] = fitness;
            }

            //TODO perform crossover
            return structuresFitness.OrderByDescending(s => s.Value).First().Key;
        }

        private int CalculateGeneratedStructuresFitness(GeneratedStructures model)
        {
            var fitness = 0;
            foreach (var road in model.NewRoads)
            {
                var roads = new List<IRoad>(Settlement.Genes) { road };

                fitness += road.Buildings.Sum(b => b.GetFitness(new BuildingRule()
                {
                    Fields = Settlement.Fields,
                    Roads = roads,
                    BuildingRoad = road,
                    SettlementCenter = Settlement.SettlementCenter

                }));
            }

            foreach (var building in model.NewBuildings)
            {
                var roads = new List<IRoad>(Settlement.Genes);

                fitness += building.GetFitness(new BuildingRule()
                {
                    Fields = Settlement.Fields,
                    Roads = roads,
                    BuildingRoad = model.RoadToAttachNewBuildings,
                    SettlementCenter = Settlement.SettlementCenter
                });
            }

            return fitness;
        }
    }
}