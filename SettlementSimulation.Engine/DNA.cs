using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Models.Buildings;

namespace SettlementSimulation.Engine
{
    public class Dna : ICopyable<Dna>
    {
        #region fields
        private readonly Field[,] _fields;
        private readonly List<Point> _mainRoad;
        #endregion

        #region properties
        public List<IRoad> Genes { get; set; }
        public float Fitness { get; private set; }
        public Point SettlementCenter =>
            new Point((int)Genes.Average(g => g.Center.X), (int)Genes.Average(g => g.Center.Y));
        #endregion

        public Dna(
            Field[,] fields,
            IEnumerable<Point> mainRoad,
            bool shouldInitGenes = true)
        {
            _fields = fields;
            _mainRoad = mainRoad.ToList();
            _mainRoad.ForEach(p => _fields[p.X, p.Y].IsBlocked = true);
            Genes = new List<IRoad>();

            if (!shouldInitGenes) return;
            InitializeGenes();
        }

        private void InitializeGenes()
        {
            var minRadius = _fields.GetLength(0) / 100 < 10 ? 10 : _fields.GetLength(0) / 100;
            var maxRadius = _fields.GetLength(0) / 10 < 10 ? 10 : _fields.GetLength(0) / 10;
            var settlementFields = _fields.ToList()
                .Where(f => f.InSettlement &&
                            f.Position.X > maxRadius &&
                            f.Position.X < _fields.GetLength(0) - maxRadius &&
                            f.Position.Y > maxRadius &&
                            f.Position.Y < _fields.GetLength(1) - maxRadius)
                .ToList();

            Point center = new Point(-1, -1);
            int radius = -1;
            for (int r = maxRadius; r >= minRadius; r--)
            {
                Field centerField = settlementFields
                    .FirstOrDefault(f =>
                        f.Position.GetCircularPoints(r, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 2.0, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 4.0, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 6.0, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 8.0, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement) &&
                        f.Position.GetCircularPoints(r / 10.0, Math.PI / 17.0f)
                            .All(p => _fields[p.X, p.Y].InSettlement));

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
            var initialRoadsCount = 3;

            var initialRoads = new List<IRoad>(initialRoadsCount);
            var firstRoadPoints = roadGenerator.GenerateStraight(new RoadGenerationTwoPoints()
            {
                Start = new Point(center.X - radius / 2, center.Y),
                End = new Point(center.X + radius / 2, center.Y),
                Fields = _fields
            });
            initialRoads.Add(new Road(firstRoadPoints));
            AddRoad(initialRoads.First());

            while (initialRoads.Count != initialRoadsCount)
            {
                var roadToAttach = initialRoads[RandomProvider.Next(initialRoads.Count)];
                var roadPoints = roadGenerator.GenerateAttached(new RoadGenerationAttached()
                {
                    Road = roadToAttach,
                    Roads = initialRoads,
                    Fields = _fields
                }).ToList();

                if (!roadPoints.Any()) continue;

                var newRoad = new Road(roadPoints);
                initialRoads.Add(newRoad);
                AddRoad(newRoad);

                while (newRoad.Buildings.Count < 0.25 * newRoad.Length)
                {
                    var building = CreateNewBuilding(newRoad, Epoch.First);
                    AddBuildingToRoad(newRoad, building);
                }
            }
        }

        public float CalculateFitness(Epoch epoch, int generation)
        {
            //TODO
            return 0;
        }

        public Dna Crossover(Dna otherParent, Epoch epoch)
        {
            return this;
        }

        public void Mutate(Epoch epoch, float mutationRate = 0.01F)
        {
            //TODO
        }

        public IEnumerable<ISettlementStructure> AddNewSettlementStructure(Epoch epoch, Action setNextEpoch)
        {
            var newStructures = new List<ISettlementStructure>();
            if (Genes.Sum(g => g.Length) < 1.5 * EpochSpecific.GetBuildingsCount(epoch))
            {
                var genes = this.Genes
                    .Where(g => g.GetPossiblePositionsToAttachRoad(new List<IRoad>(this.Genes)).Count > 1)
                    .ToList();

                if (RandomProvider.NextDouble() <= 0.9) //in order to make it more probable for roads closer to center to be selected
                {
                    var numberOfGenesToInclude = 0.15 * genes.Count <= 1 ? 1 : (int)(0.1 * genes.Count);
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
                    return newStructures;

                this.AddRoad(road);

                while (road.Buildings.Count < 0.5 * road.Length)
                {
                    var building = CreateNewBuilding(road, epoch);
                    AddBuildingToRoad(road, building);
                }

                newStructures.Add(road);
                return newStructures;
            }
            else if (Genes.Sum(g => g.Buildings.Count) < EpochSpecific.GetBuildingsCount(epoch))
            {
                var roadsToAttach = this.Genes
                    .Where(g => g.Buildings.Count < g.Length)
                    .ToArray();

                var roadToAttach = roadsToAttach[RandomProvider.Next(roadsToAttach.Count())];

                while (roadToAttach.Buildings.Count < roadToAttach.Length)
                {
                    var building = this.CreateNewBuilding(roadToAttach, epoch);
                    AddBuildingToRoad(roadToAttach, building);
                    newStructures.Add(building);

                }
                return newStructures;
            }
            else if (epoch != Epoch.Third)
            {
                setNextEpoch.Invoke();
            }

            return newStructures;
        }

        public Dna Copy()
        {
            var copy = new Dna(_fields, _mainRoad, false);
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
                Fields = this._fields,
                SettlementCenter = this.SettlementCenter,
                MinDistanceBetweenRoads = 10,
                MinRoadLength = 5,
                MaxRoadLength = 100
            }).ToList();

            return new Road(roadPoints);
        }

        private IBuilding CreateNewBuilding(IRoad road, Epoch epoch)
        {
            var positions = road.GetPossiblePositionsToAttachBuilding(this.Genes);
            if (!positions.Any())
                return null;

            var building = Building.GetRandom(epoch);
            building.Position = positions[RandomProvider.Next(positions.Count)];

            return building;
        }

        public void AddRoad(IRoad road)
        {
            if (!CanAddRoad(road)) return;

            foreach (var segment in road.Segments)
            {
                _fields[segment.Position.X, segment.Position.Y].IsBlocked = true;
            }

            foreach (var building in road.Buildings)
            {
                _fields[building.Position.X, building.Position.Y].IsBlocked = true;
            }
            this.Genes.Add(road);
        }

        public void AddBuildingToRoad(IRoad road, IBuilding building)
        {
            if (road.AddBuilding(building))
            {
                _fields[building.Position.X, building.Position.Y].IsBlocked = true;
            }
        }

        public bool CanAddRoad(IRoad road)
        {
            if (!road.Segments.Any())
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


            if (road.Segments.Any(s => !this._fields[s.Position.X, s.Position.Y].InSettlement ||
                                       (this._fields[s.Position.X, s.Position.Y].IsBlocked.HasValue &&
                                        this._fields[s.Position.X, s.Position.Y].IsBlocked.Value)))
            {
                return false;
            }

            if (road.Buildings.Any(b => !this._fields[b.Position.X, b.Position.Y].InSettlement ||
                                        (this._fields[b.Position.X, b.Position.Y].IsBlocked.HasValue &&
                                         this._fields[b.Position.X, b.Position.Y].IsBlocked.Value)))
            {
                return false;
            }

            return true;
        }
    }
}