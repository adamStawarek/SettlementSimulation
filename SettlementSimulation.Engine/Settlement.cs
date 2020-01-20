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
        public List<IRoad> Genes { get; set; }
        public float Fitness { get; private set; }
        public Point SettlementCenter =>
            new Point((int)Genes.Average(g => g.Center.X), (int)Genes.Average(g => g.Center.Y));
        public List<IBuilding> Buildings => Genes.SelectMany(g => g.Buildings).ToList();
        public Point SettlementUpperLeftBound =>
            new Point(this.Genes.Min(g => g.Start.X), this.Genes.Min(g => g.Start.Y));
        public Point SettlementBottomRightBound =>
            new Point(this.Genes.Max(g => g.Start.X), this.Genes.Max(g => g.Start.Y));
        #endregion

        public Settlement(
            Field[,] fields,
            IEnumerable<Point> mainRoad,
            bool shouldInitGenes = true)
        {
            Fields = fields;
            MainRoad = mainRoad.ToList();
            MainRoad.ForEach(p => Fields[p.X, p.Y].IsBlocked = true);
            Genes = new List<IRoad>();

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
                    firstRoad.GetPossibleBuildingPositions(new PossibleBuildingPositions(this.Genes, this.Fields));
                var building = Building.GetRandom(Epoch.First);
                building.Position = possiblePlaces[RandomProvider.Next(possiblePlaces.Count)];
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
                    var building = Building.GetRandom(Epoch.First);
                    var possiblePlaces =
                        newRoad.GetPossibleBuildingPositions(new PossibleBuildingPositions(this.Genes, Fields));

                    building.Position = possiblePlaces[RandomProvider.Next(possiblePlaces.Count)];
                    newRoad.AddBuilding(building);
                }

                if (CanAddRoad(newRoad))
                    AddRoad(newRoad);
            }
        }

        public SettlementUpdate CreateNewSettlementUpdate(Epoch epoch)
        {
            var generatedStructures = new SettlementUpdate();
            if (!EpochSpecific.IsSatisfiedBuildingCountCondition(this, epoch))
            {
                if (this.Genes.Sum(g => g.Length) < 1.5 * EpochSpecific.GetBuildingsCount(epoch))
                {
                    var genes = this.Genes.ToList();

                    if (RandomProvider.NextDouble() < 0.3) //in order to make it more probable for roads closer to center to be selected
                    {
                        var numberOfGenesToInclude = (int)(0.2 * genes.Count) <= 1 ? 1 : (int)(0.1 * genes.Count);
                        genes = genes.OrderBy(g =>
                                g.IsVertical
                                    ? Math.Abs(g.Start.X - SettlementCenter.X)
                                    : Math.Abs(g.Start.Y - SettlementCenter.Y))
                            .Take(2 * numberOfGenesToInclude)
                            .ToList();
                        genes = genes
                            .OrderBy(g => g.AttachedRoads(new List<IRoad>(this.Genes)).Count)
                            .Take(numberOfGenesToInclude)
                            .ToList();
                    }

                    var roadToAttach = genes[RandomProvider.Next(genes.Count)];
                    var road = this.CreateNewRoad(roadToAttach);
                    if (!CanAddRoad(road))
                        return generatedStructures;

                    var possiblePlaces =
                        road.GetPossibleBuildingPositions(new PossibleBuildingPositions(this.Genes, Fields));

                    var buildingsToAdd = possiblePlaces.Count / 3 < 1 ? possiblePlaces.Count : possiblePlaces.Count / 3;

                    for (int i = 0; i < buildingsToAdd; i++)
                    {
                        var building = Building.GetRandom(epoch);
                        building.Position = possiblePlaces[RandomProvider.Next(possiblePlaces.Count)];
                        road.AddBuilding(building);
                    }

                    generatedStructures.NewRoads.Add(road);
                    return generatedStructures;
                }
                else
                {
                    var roadsToAttach = this.Genes
                        .Where(g => g.Buildings.Count < g.Length)
                        .ToArray();

                    var roadToAttach = roadsToAttach[RandomProvider.Next(roadsToAttach.Count())];
                    var copy = roadToAttach.Copy();

                    var possiblePlaces =
                        copy.GetPossibleBuildingPositions(new PossibleBuildingPositions(this.Genes, Fields));
                    var buildingsToAdd = possiblePlaces.Count / 2 < 1 ? possiblePlaces.Count : possiblePlaces.Count / 2;

                    for (int i = 0; i < buildingsToAdd; i++)
                    {
                        var building = Building.GetRandom(epoch);
                        building.Position = possiblePlaces[RandomProvider.Next(possiblePlaces.Count)];
                        copy.AddBuilding(building);
                        generatedStructures.NewBuildingsAttachedToRoad.Add((building, roadToAttach));
                    }
                    return generatedStructures;
                }
            }
            else
            {
                var numberOfBuildingsToUpdate = 10;
                for (int i = 0; i < numberOfBuildingsToUpdate; i++)
                {
                    var roadsWithBuildings = this.Genes.Where(g => g.Buildings.Any()).ToList();
                    var road = roadsWithBuildings[RandomProvider.Next(roadsWithBuildings.Count)];
                    var building = road.Buildings[RandomProvider.Next(road.Buildings.Count)];

                    if (!(building is Residence) && building.CalculateFitness(new BuildingRule()
                    {
                        Fields = this.Fields,
                        SettlementCenter = this.SettlementCenter,
                        BuildingRoad = road,
                        Roads = this.Genes
                    }) != 0) continue;

                    var newBuilding = Building.GetRandom(epoch);
                    newBuilding.Position = building.Position;
                    newBuilding.Direction = building.Direction;

                    generatedStructures.BuildingRemovedFromRoad.Add((building, road));
                    generatedStructures.NewBuildingsAttachedToRoad.Add((newBuilding, road));
                }

                return generatedStructures;
            }
        }

        public Settlement Copy()
        {
            var copy = new Settlement(Fields, MainRoad, false);
            Genes.Cast<ICopyable<Road>>().ToList().ForEach(g => copy.Genes.Add(g.Copy()));
            copy.Fitness = this.Fitness;
            return copy;
        }

        private IRoad CreateNewRoad(IRoad road)
        {
            var roadGenerator = new RoadPointsGenerator();
            var roadPoints = roadGenerator.GenerateAttached(new RoadGenerationAttached()
            {
                Road = road,
                Roads = this.Genes,
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
            if (road.AddBuilding(building))
            {
                Fields[building.Position.X, building.Position.Y].IsBlocked = true;
            }
        }

        public void RemoveBuildingFromRoad(IRoad road, IBuilding building)
        {
            if (road.RemoveBuilding(building))
            {
                Fields[building.Position.X, building.Position.Y].IsBlocked = false;
            }
        }

        public void RemoveRoad(IRoad road)
        {
            foreach (var segment in road.Segments)
            {
                Fields[segment.Position.X, segment.Position.Y].IsBlocked = false;
            }

            foreach (var building in road.Buildings)
            {
                Fields[building.Position.X, building.Position.Y].IsBlocked = false;
            }
            this.Genes.Remove(road);
        }

        public void AddRoad(IRoad road)
        {
            foreach (var segment in road.Segments)
            {
                Fields[segment.Position.X, segment.Position.Y].IsBlocked = true;
            }

            foreach (var building in road.Buildings)
            {
                Fields[building.Position.X, building.Position.Y].IsBlocked = true;
            }
            this.Genes.Add(road);
        }

        public bool CanAddRoad(IRoad road)
        {
            if (!road.Segments.Any())
                return false;

            if (road.Length < MinRoadLength)
                return false;

            if (road.IsVertical)
            {
                if (this.Genes.Where(g => g.IsVertical).Any(g => Math.Abs(g.Start.X - road.Start.X) <= 2 &&
                                                                 g.Segments.Any(s => road.Segments.Any(r => r.Position.Y == s.Position.Y))))
                    return false;
            }
            else
            {
                if (this.Genes.Where(g => !g.IsVertical).Any(g => Math.Abs(g.Start.Y - road.Start.Y) <= 2 &&
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

        public MutationResult InvokeFloodMutation()
        {
            var dangerZone = 5;
            var roadsNearWater = this.Genes.Where(g =>
                this.Fields[g.Start.X, g.Start.Y].DistanceToWater <= dangerZone ||
                this.Fields[g.End.X, g.End.Y].DistanceToWater <= dangerZone).ToList();

            roadsNearWater.ForEach(this.RemoveRoad);

            return new MutationResult() { RemovedRoads = roadsNearWater };
        }

        public MutationResult InvokeEarthquakeMutation()
        {
            var result = new MutationResult();
            var buildingsToRemove = RandomProvider.Next((int)(this.Buildings.Count * 0.1), (int)(this.Buildings.Count * 0.2));
            for (int i = 0; i < buildingsToRemove; i++)
            {
                var road = this.Genes[RandomProvider.Next(this.Genes.Count)];
                if (!road.Buildings.Any()) continue;
                var building = road.Buildings[RandomProvider.Next(road.Buildings.Count)];
                this.RemoveBuildingFromRoad(road, building);
                result.RemovedBuildings.Add(building);
            }

            return result;
        }
    }
}