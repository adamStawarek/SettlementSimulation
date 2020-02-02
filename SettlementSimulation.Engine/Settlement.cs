using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Models.Buildings;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using static SettlementSimulation.Engine.Helpers.ConfigurationManager;

namespace SettlementSimulation.Engine
{
    public class Settlement : ICopyable<Settlement>
    {
        #region properties
        public Field[,] Fields { get; }
        public List<Point> MainRoad { get; }
        public List<IRoad> Roads { get; set; }
        public float Fitness { get; private set; }
        public Point SettlementCenter { get; set; }
        public List<IBuilding> Buildings => Roads.SelectMany(g => g.Buildings).ToList();
        #endregion

        public Settlement(
            Field[,] fields,
            IEnumerable<Point> mainRoad,
            bool shouldInitGenes = true)
        {
            Fields = fields;
            MainRoad = mainRoad.ToList();
            MainRoad.ForEach(p => Fields[p.X, p.Y].IsBlocked = true);
            Roads = new List<IRoad>();

            if (!shouldInitGenes) return;
            InitializeGenes();
        }

        private void InitializeGenes()
        {
            var minRadius = Fields.GetLength(0) / 100 < 10 ? 10 : Fields.GetLength(0) / 100;
            var maxRadius = Fields.GetLength(0) / 10 < 10 ? 10 : Fields.GetLength(0) / 10;
            var settlementFields = Fields.ToList()
                .Where(f => f.InSettlement &&
                            f.Position.X > maxRadius &&
                            f.Position.X < Fields.GetLength(0) - maxRadius &&
                            f.Position.Y > maxRadius &&
                            f.Position.Y < Fields.GetLength(1) - maxRadius)
                .ToList();

            Point center = new Point(-1, -1);
            int radius = -1;
            for (int r = maxRadius; r >= minRadius; r--)
            {
                Field centerField = settlementFields
                    .FirstOrDefault(f =>
                        f.Position.GetCircularPoints(r, Math.PI / 17.0f)
                            .All(p => Fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 2.0, Math.PI / 17.0f)
                            .All(p => Fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 4.0, Math.PI / 17.0f)
                            .All(p => Fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 6.0, Math.PI / 17.0f)
                            .All(p => Fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 8.0, Math.PI / 17.0f)
                            .All(p => Fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 10.0, Math.PI / 17.0f)
                            .All(p => Fields[p.X, p.Y].InSettlement));

                if (centerField != null)
                {
                    radius = r;
                    center = centerField.Position;
                    break;
                }

                if (r == minRadius)
                {
                    throw new Exception("Cannot find center for initial point");
                }
            }

            SettlementCenter = center;

            var roadGenerator = new RoadPointsGenerator();

            var initialRoads = new List<IRoad>(InitialRoadsCount);
            var firstRoadPoints = roadGenerator.GenerateStraight(new RoadGenerationTwoPoints()
            {
                Start = new Point(center.X - radius / 2, center.Y),
                End = new Point(center.X + radius / 2, center.Y),
                Fields = Fields
            });
            var firstRoad = new Road(firstRoadPoints);
            while (firstRoad.Buildings.Count < 0.25 * firstRoad.Length)
            {
                var possiblePlaces =
                    firstRoad.GetPossibleBuildingPositions(new PossibleBuildingPositions(this.Roads, this.Fields));
                var building = new Residence
                {
                    Position = possiblePlaces[RandomProvider.Next(possiblePlaces.Count)],
                    Road = firstRoad,
                    Fitness = 0.1,
                    Material = Material.Wood
                };
                firstRoad.AddBuilding(building);
            }
            initialRoads.Add(firstRoad);
            AddRoad(initialRoads.First());

            while (initialRoads.Count != InitialRoadsCount)
            {
                var roadToAttach = initialRoads[RandomProvider.Next(initialRoads.Count)];
                var roadPoints = roadGenerator.GenerateAttached(new RoadGenerationAttached()
                {
                    Road = roadToAttach,
                    Roads = initialRoads,
                    Fields = Fields,
                    MaxRoadLength = MaxRoadLength,
                    MinRoadLength = MinRoadLength,
                    MinDistanceBetweenRoads = MinDistanceBetweenRoads
                }).ToList();

                if (!roadPoints.Any()) continue;

                var newRoad = new Road(roadPoints);
                initialRoads.Add(newRoad);

                while (newRoad.Buildings.Count < 0.25 * newRoad.Length)
                {
                    var possiblePlaces =
                        newRoad.GetPossibleBuildingPositions(new PossibleBuildingPositions(this.Roads, Fields));

                    var building = new Residence
                    {
                        Position = possiblePlaces[RandomProvider.Next(possiblePlaces.Count)],
                        Road = newRoad,
                        Fitness = 0.1,
                        Material =Material.Wood
                    };
                    newRoad.AddBuilding(building);
                }

                if (CanAddRoad(newRoad))
                    AddRoad(newRoad);
            }
        }

        public SettlementUpdate CreateNewSettlementUpdate(
            UpdateType updateType,
            Epoch epoch,
            int iteration,
            int maxIterations)
        {
            var roadTypeSetUp = new RoadTypeSetUp()
            {
                Epoch = epoch,
                SettlementCenter = SettlementCenter,
                AvgDistanceToSettlementCenter =
                    (int)Roads.Average(r => r.Center.DistanceTo(SettlementCenter))
            };
            SettlementUpdate settlementUpdate = new SettlementUpdate(updateType);
            switch (updateType)
            {
                case UpdateType.NewRoads:
                    {
                        var genes = this.Roads.ToList();
                        
                        if (RandomProvider.NextDouble() < 0.5) //in order to make it more probable for roads closer to center to be selected
                        {
                            var numberOfGenesToInclude = (int)(0.2 * genes.Count) <= 1 ? 1 : (int)(0.2 * genes.Count);
                            genes = genes.OrderBy(g => g.Center.DistanceTo(this.SettlementCenter))
                                .Take(2 * numberOfGenesToInclude)
                                .ToList();
                        }
                        var roadToAttach = genes[RandomProvider.Next(genes.Count)];
                        var road = this.CreateNewRoad(roadToAttach);
                        if (!CanAddRoad(road))
                            return settlementUpdate;

                        var possiblePlaces =
                            road.GetPossibleBuildingPositions(new PossibleBuildingPositions(this.Roads, Fields));

                        var buildingsToAdd = RandomProvider.Next(1,
                            possiblePlaces.Count > MaxBuildingsToAddPerIteration / 2
                                ? MaxBuildingsToAddPerIteration / 2
                                : possiblePlaces.Count);

                        for (int i = 0; i < buildingsToAdd; i++)
                        {
                            var building = Building.GetRandom(epoch);
                            building.Position = possiblePlaces[RandomProvider.Next(possiblePlaces.Count)];
                            building.Road = road;
                            building.Material = EpochSpecific.GetMaterialForBuilding(epoch);
                            road.AddBuilding(building);
                        }

                        road.SetUpRoadType(roadTypeSetUp);
                        settlementUpdate.NewRoads.Add(road);
                        return settlementUpdate;
                    }
                case UpdateType.NewBuildings:
                    {
                        var roadWithoutBuildings = this.Roads.FirstOrDefault(g => !g.Buildings.Any());

                        var roadsToAttachCount = this.Roads.Count * 0.3 < 10 ? 10 : (int)(this.Roads.Count * 0.3);
                        var roadsToAttach = this.Roads
                            .OrderByDescending(g => 2 * g.Length - g.Buildings.Count)
                            .Take(roadsToAttachCount)
                            .ToArray();

                        var roadToAttach = roadWithoutBuildings ?? roadsToAttach[RandomProvider.Next(roadsToAttach.Count())];
                        var copy = roadToAttach.Copy();

                        var possiblePlaces =
                            copy.GetPossibleBuildingPositions(new PossibleBuildingPositions(this.Roads, Fields));
                        if (!possiblePlaces.Any()) return settlementUpdate;

                        var buildingsToAdd = RandomProvider.Next(1,
                            possiblePlaces.Count > MaxBuildingsToAddPerIteration
                                ? MaxBuildingsToAddPerIteration
                                : possiblePlaces.Count);

                        double coef = (3 * iteration / (maxIterations * 0.1));
                        buildingsToAdd = (int)(coef * buildingsToAdd);

                        for (int i = 0; i < buildingsToAdd; i++)
                        {
                            var building = Building.GetRandom(epoch);
                            building.Position = possiblePlaces[RandomProvider.Next(possiblePlaces.Count)];
                            building.Road = roadToAttach;
                            building.Material = EpochSpecific.GetMaterialForBuilding(epoch);
                            copy.AddBuilding(building);
                            settlementUpdate.NewBuildings.Add(building);
                        }
                        return settlementUpdate;
                    }
                default:
                    for (int i = 0; i < BuildingsPerUpdate; i++)
                    {
                        var roadsWithBuildings = this.Roads.Where(g => g.Buildings.Any()).ToList();
                        var road = roadsWithBuildings[RandomProvider.Next(roadsWithBuildings.Count)];
                        var building = road.Buildings[RandomProvider.Next(road.Buildings.Count)];

                        if (!(building is Residence) && Math.Abs(building.Fitness.Value) > 0.1) continue;//don't update rare buildings with positive fitness

                        var newBuilding = Building.GetRandom(epoch);
                        newBuilding.Position = building.Position;
                        newBuilding.Direction = building.Direction;
                        newBuilding.Road = building.Road;
                        newBuilding.Material = EpochSpecific.GetMaterialForBuilding(epoch);

                        if(building.GetType() != newBuilding.GetType() || building.Material != newBuilding.Material)
                            settlementUpdate.UpdatedBuildings.Add((building, newBuilding));
                    }

                    for (int i = 0; i < RoadsPerUpdate; i++)
                    {
                        var unpavedRoads = this.Roads.Where(g => g.Type == RoadType.Unpaved).ToList();
                        if (!unpavedRoads.Any()) break;
                        var oldRoad = unpavedRoads[RandomProvider.Next(unpavedRoads.Count)];
                        var newRoad = oldRoad.Copy();
                        newRoad.SetUpRoadType(roadTypeSetUp);
                        if (!oldRoad.Type.Equals(newRoad.Type))
                            settlementUpdate.UpdatedRoads.Add((oldRoad, newRoad));
                    }
                    return settlementUpdate;
            }
        }
        
        public Settlement Copy()
        {
            var copy = new Settlement(Fields, MainRoad, false);
            Roads.Cast<ICopyable<Road>>().ToList().ForEach(g => copy.Roads.Add(g.Copy()));
            copy.Fitness = this.Fitness;
            return copy;
        }

        private IRoad CreateNewRoad(IRoad road)
        {
            var roadGenerator = new RoadPointsGenerator();
            var roadPoints = roadGenerator.GenerateAttached(new RoadGenerationAttached()
            {
                Road = road,
                Roads = this.Roads,
                Fields = this.Fields,
                SettlementCenter = this.SettlementCenter,
                MinDistanceBetweenRoads = MinDistanceBetweenRoads,
                MinRoadLength = MinRoadLength,
                MaxRoadLength = MaxRoadLength
            }).ToList();

            return new Road(roadPoints);
        }

        public void AddBuildingToRoad(IRoad road, IBuilding building)
        {
            if (road != building.Road)
            {
                throw new Exception("Road and building road are not the same");
            }
            if (road.AddBuilding(building))
            {
                Fields[building.Position.X, building.Position.Y].IsBlocked = true;
            }
        }

        public void RemoveBuildingFromRoad(IRoad road, IBuilding building)
        {
            if (road != building.Road)
            {
                throw new Exception("Road and building road are not the same");
            }
            if (road.RemoveBuilding(building))
            {
                Fields[building.Position.X, building.Position.Y].IsBlocked = false;
            }
        }

        public void AddRoad(IRoad road)
        {
            if (road.Buildings.Any(b => b.Road != road))
            {
                throw new Exception("Road and building road are not the same");
            }

            foreach (var segment in road.Segments)
            {
                Fields[segment.Position.X, segment.Position.Y].IsBlocked = true;
            }

            foreach (var building in road.Buildings)
            {
                Fields[building.Position.X, building.Position.Y].IsBlocked = true;
            }
            this.Roads.Add(road);
        }

        public bool CanAddRoad(IRoad road)
        {
            if (!road.Segments.Any())
                return false;

            if (road.Length < MinRoadLength)
                return false;

            if (road.IsVertical)
            {
                if (this.Roads.Where(g => g.IsVertical).Any(g => Math.Abs(g.Start.X - road.Start.X) <= 2 &&
                                                                 g.Segments.Any(s => road.Segments.Any(r => r.Position.Y == s.Position.Y))))
                    return false;
            }
            else
            {
                if (this.Roads.Where(g => !g.IsVertical).Any(g => Math.Abs(g.Start.Y - road.Start.Y) <= 2 &&
                                                                  g.Segments.Any(s => road.Segments.Any(r => r.Position.X == s.Position.X))))
                    return false;
            }


            if (road.Segments.Any(s => !this.Fields[s.Position.X, s.Position.Y].InSettlement ||
                                       (this.Fields[s.Position.X, s.Position.Y].IsBlocked.HasValue &&
                                        this.Fields[s.Position.X, s.Position.Y].IsBlocked.Value)))
            {
                return false;
            }

            if (road.Buildings.Any(b => !this.Fields[b.Position.X, b.Position.Y].InSettlement ||
                                        (this.Fields[b.Position.X, b.Position.Y].IsBlocked.HasValue &&
                                         this.Fields[b.Position.X, b.Position.Y].IsBlocked.Value)))
            {
                return false;
            }

            return true;
        }

        public void UpdateSettlementCenter()
        {
            SettlementCenter = new Point((int)Roads.Average(g => g.Center.X), (int)Roads.Average(g => g.Center.Y));
        }

        public MutationResult InvokeFloodMutation()
        {
            var buildingsNearWater = this.Roads.SelectMany(r => r.Buildings)
                .Where(b => this.Fields[b.Position.X, b.Position.Y].DistanceToWater <= FloodMutationDistanceToWater)
                .ToList();

            buildingsNearWater.ForEach(b => RemoveBuildingFromRoad(b.Road, b));

            return new MutationResult() { RemovedBuildings = buildingsNearWater };
        }

        public MutationResult InvokeEarthquakeMutation()
        {
            var result = new MutationResult();
            var buildingsToRemove = RandomProvider.Next((int)(this.Buildings.Count * 0.1));
            for (int i = 0; i < buildingsToRemove; i++)
            {
                var road = this.Roads[RandomProvider.Next(this.Roads.Count)];
                if (!road.Buildings.Any()) continue;
                var building = road.Buildings[RandomProvider.Next(road.Buildings.Count)];
                this.RemoveBuildingFromRoad(road, building);
                result.RemovedBuildings.Add(building);
            }

            return result;
        }

        public MutationResult InvokeFireMutation()
        {
            var result = new MutationResult();
            var woodenBuildings = this.Buildings.Where(b => b.Material.Value == Material.Wood).ToList();
            if (!woodenBuildings.Any()) return result;

            var buildingsToRemove = new Stack<IBuilding>();
            var first = woodenBuildings[RandomProvider.Next(woodenBuildings.Count)];
            buildingsToRemove.Push(first);
            woodenBuildings.Remove(first);
            result.RemovedBuildings.Add(first);

            while (buildingsToRemove.Any())
            {
                var building = buildingsToRemove.Pop();
                var closeBuildings = woodenBuildings.Where(b => b.Position.DistanceTo(building.Position) <= 3).ToList();
                woodenBuildings = woodenBuildings.Except(closeBuildings).ToList();

                result.RemovedBuildings.AddRange(closeBuildings);
                closeBuildings.ForEach(b => buildingsToRemove.Push(b));
            }

            return result;
        }
    }
}