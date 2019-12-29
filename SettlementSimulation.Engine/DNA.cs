using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine
{
    public class Dna
    {
        #region
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
            List<Point> mainRoad,
            bool shouldInitGenes = true)
        {
            _fields = fields;
            _mainRoad = mainRoad;
            Genes = new List<IRoad>();
            _ruleDistributor = new RuleDistributor();

            if (!shouldInitGenes) return;
            InitializeGenes();
        }

        private void InitializeGenes()
        {
            var radius = 15;
            var avgDistanceToWaterAndMainRoad = _fields.ToList()
                .Where(f => f.InSettlement)
                .Average(f => f.DistanceToMainRoad + f.DistanceToWater);

            var center = _fields.ToList()
                .Where(f => f.InSettlement &&
                            f.Position.X < _fields.GetLength(0) - 3 * radius &&
                            f.Position.X > 3 * radius &&
                            f.Position.Y < _fields.GetLength(1) - 3 * radius &&
                            f.Position.Y > 3 * radius &&
                            f.DistanceToMainRoad + f.DistanceToWater <= avgDistanceToWaterAndMainRoad &&
                            f.Position.GetCircularPoints(radius, Math.PI / 17.0f)
                                .All(p => _fields[p.X, p.Y].InSettlement)&&
                            f.Position.GetCircularPoints(2*radius, Math.PI / 17.0f)
                                .All(p => _fields[p.X, p.Y].InSettlement))
                .Select(p => p.Position)
                .First();

            var edgePoints = new List<Point>();
            while (edgePoints.Count() != 5)
            {
                var randX = RandomProvider.Next(center.X - 2 * radius, center.X + 2 * radius);
                var randY = RandomProvider.Next(center.Y - 2 * radius, center.Y + 2 * radius);

                var point = new Point(randX, randY);
                if (randX < 0 || randX >= _fields.GetLength(0) ||
                    randY < 0 || randY >= _fields.GetLength(1) ||
                    _fields[point.X, point.Y].InSettlement == false ||
                    (Math.Abs(center.X - point.X) < radius &&
                    Math.Abs(center.Y - point.Y) < radius))
                {
                    continue;
                }

                if (edgePoints.All(p =>
                    Math.Abs(p.X - point.X) > 1 &&
                    Math.Abs(p.Y - point.Y) > 1 &&
                    p.DistanceTo(point) > 10))
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
            foreach (var r in roads)
            {
                var roadPoints = roadGenerator.Generate(new RoadGenerationInfo()
                { Start = r.start, End = r.end, Fields = _fields, Structures = new IBuilding[] { } });
                Genes.Add(new Road(roadPoints));
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
            var road = this.Genes[RandomProvider.Next(this.Genes.Count)];
            var segment = road.Segments[RandomProvider.Next(0, road.Segments.Count)];
            if (segment.Buildings.Any()) return this;

            var buildingPosition = segment.Position;
            if (road.Segments.Any(
                s => s.Position.Equals(new Point(segment.Position.X + 1, segment.Position.Y))))
            {
                buildingPosition.Y += 1;
            }
            else
            {
                buildingPosition.X += 1;
            }

            segment.Buildings.Add(new Residence() { Position = buildingPosition });

            return this;
        }

        public void Mutate(Epoch epoch, float mutationRate = 0.01F)
        {
            //TODO
        }
    }
}