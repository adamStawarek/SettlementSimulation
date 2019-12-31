using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Models.Buildings;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine
{
    public class Dna : ICopyable<Dna>
    {
        #region fields
        private readonly Field[,] _fields;
        private readonly List<Point> _mainRoad;
        private readonly IRuleDistributor _ruleDistributor;
        #endregion

        #region properties
        public List<IRoad> Genes { get; set; }
        public float Fitness { get; private set; }
        #endregion

        public Dna(
            Field[,] fields,
            IEnumerable<Point> mainRoad,
            bool shouldInitGenes = true)
        {
            _fields = fields;
            _mainRoad = mainRoad.ToList();
            Genes = new List<IRoad>();
            _ruleDistributor = new RuleDistributor();

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

            var edgePoints = new List<Point>();
            while (edgePoints.Count() != 5)
            {
                var randX = RandomProvider.Next(center.X - radius, center.X + radius);
                var randY = RandomProvider.Next(center.Y - radius, center.Y + radius);

                var point = new Point(randX, randY);
                if (edgePoints.All(p =>
                    Math.Abs(p.X - point.X) > 1 &&
                    Math.Abs(p.Y - point.Y) > 1 &&
                    p.DistanceTo(point) > radius / 2.0))
                {
                    edgePoints.Add(point);
                }
            }

            var roads = new List<(Point start, Point end)>();
            var maxX = edgePoints.Max(p => p.X);
            var minX = edgePoints.Min(p => p.X);
            var maxY = edgePoints.Max(p => p.Y);
            var minY = edgePoints.Min(p => p.Y);
            roads.Add((new Point(minX, minY), new Point(maxX, minY)));
            roads.Add((new Point(maxX, minY), new Point(maxX, maxY)));
            roads.Add((new Point(maxX, maxY), new Point(minX, maxY)));
            roads.Add((new Point(minX, maxY), new Point(minX, minY)));
            foreach (var point in edgePoints)
            {
                if (point.X < maxX && point.X > minX)
                {
                    //add vertical road
                    roads.Add((new Point(point.X, minY), new Point(point.X, maxY)));
                }

                if (point.Y < maxY && point.Y > minY)
                {
                    roads.Add((new Point(minX, point.Y), new Point(maxX, point.Y)));
                    //add horizontal road
                }
            }

            var roadGenerator = new RoadGenerator();
            foreach (var road in roads)
            {
                var roadPoints = roadGenerator.Generate(new RoadGenerationTwoPoints()
                { Start = road.start, End = road.end, Fields = _fields }).ToList();
                if (roadPoints.Any())
                {
                    Genes.Add(new Road(roadPoints));
                }
            }

            foreach (var g1 in Genes)
            {
                foreach (var g2 in Genes)
                {
                    if (g1.Equals(g2)) continue;
                    g1.BlockCell(g2.Start);
                    g1.BlockCell(g2.End);
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
            //TODO join this parts of the dna's that don't overlap
            var dna = this.Copy();
            var road = dna.Genes[RandomProvider.Next(dna.Genes.Count)];

            if (road.Buildings.Count > 0.5 * road.Length)
            {
                var roadGenerator = new RoadGenerator();
                var roadPoints = roadGenerator.GenerateAttached(new RoadGenerationAttached()
                {
                    Road = road,
                    Roads = dna.Genes,
                    Fields = dna._fields,
                    BlockedCells = dna.Genes.SelectMany(g => g.BlockedCells).ToList()
                }).ToList();

                if (roadPoints.Any())
                {
                    dna.Genes.Add(new Road(roadPoints));
                }
            }
            else
            {
                var positions = road.GetPossiblePositionsToAttachBuilding();
                if (!positions.Any()) return dna;

                var building = Building.GetRandom(epoch);
                building.Position = positions[RandomProvider.Next(positions.Count)];
                road.AddBuilding(building);
            }

            return dna;
        }

        public void Mutate(Epoch epoch, float mutationRate = 0.01F)
        {
            //TODO
        }

        public Dna Copy()
        {
            var copy = new Dna(_fields, _mainRoad, false);
            this.Genes.Cast<ICopyable<Road>>().ToList().ForEach(g => copy.Genes.Add(g.Copy()));
            copy.Fitness = this.Fitness;
            return copy;
        }
    }
}