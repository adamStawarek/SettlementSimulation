using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using static SettlementSimulation.Engine.Helpers.ConfigurationManager;

namespace SettlementSimulation.Engine.Models
{
    public class Road : IRoad
    {
        #region
        private RoadType roadType;
        #endregion

        public Road(IEnumerable<Point> positions)
        {
            roadType = RoadType.Unpaved;
            Segments = positions
                .OrderBy(p => p.X)
                .ThenBy(p => p.Y)
                .Select(p => new RoadSegment(p))
                .ToList();
        }

        public List<RoadSegment> Segments { get; }
        public Point Start => Segments.First().Position;
        public Point End => Segments.Last().Position;
        public Point Center => new Point((Start.X + End.X) / 2, (Start.Y + End.Y) / 2);
        public int Length => Segments.Count;
        public RoadType Type => roadType;
        public bool IsVertical => Start.X.Equals(End.X);
        public List<IBuilding> Buildings => Segments.SelectMany(s => s.Buildings).ToList();

        public List<Point> GetPossibleBuildingPositions(PossibleBuildingPositions model)
        {
            var possiblePositions = new List<Point>();

            foreach (var segment in Segments)
            {
                if (segment.IsFull) continue;

                var points = new List<Point>();
                if (IsVertical)
                {
                    points.Add(new Point(segment.Position.X - 1, segment.Position.Y));
                    points.Add(new Point(segment.Position.X + 1, segment.Position.Y));
                }
                else
                {
                    points.Add(new Point(segment.Position.X, segment.Position.Y - 1));
                    points.Add(new Point(segment.Position.X, segment.Position.Y + 1));
                }

                points.RemoveAll(p => Buildings.Select(b => b.Position).Any(b => b.Equals(p)) ||
                                      (model.Fields[p.X, p.Y].IsBlocked.HasValue && model.Fields[p.X, p.Y].IsBlocked.Value) ||
                                      !model.Fields[p.X, p.Y].InSettlement ||
                                      model.Roads.Any(r => r.Start.Equals(p) || r.End.Equals(p)));

                possiblePositions.AddRange(points);
            }

            return possiblePositions;
        }

        public List<Point> GetPossibleRoadPositions(PossibleRoadPositions model)
        {
            var possiblePositions = new List<Point>();
            var roads = new List<IRoad>(model.Roads);

            roads.RemoveAll(r => r.Start.Equals(this.Start) && r.End.Equals(this.End));

            foreach (var segment in Segments)
            {
                if (segment.IsFull) continue;

                var points = new List<Point>();
                if (IsVertical)
                {
                    points.Add(new Point(segment.Position.X - 1, segment.Position.Y));
                    points.Add(new Point(segment.Position.X + 1, segment.Position.Y));
                }
                else
                {
                    points.Add(new Point(segment.Position.X, segment.Position.Y - 1));
                    points.Add(new Point(segment.Position.X, segment.Position.Y + 1));
                }

                points.RemoveAll(p => segment.Buildings.Any(b => b.Position.Equals(p)));

                foreach (var r in roads)
                {
                    if (!r.IsVertical && this.IsVertical)
                    {
                        points.RemoveAll(p => ((int)r.Start.DistanceTo(p) <= model.MinDistanceBetweenRoads && (r.Start.X == p.X) ||
                                               ((int)r.End.DistanceTo(p) <= model.MinDistanceBetweenRoads && (r.End.X == p.X))));
                    }
                    else if (r.IsVertical && !this.IsVertical)
                    {
                        points.RemoveAll(p => ((int)r.Start.DistanceTo(p) <= model.MinDistanceBetweenRoads && (r.Start.Y == p.Y) ||
                                               ((int)r.End.DistanceTo(p) <= model.MinDistanceBetweenRoads && (r.End.Y == p.Y))));
                    }
                }

                points.ForEach(p =>
                {
                    possiblePositions.Add(p);
                });
            }

            return possiblePositions;
        }

        public List<IRoad> AttachedRoads(List<IRoad> roads)
        {
            var attachedRows = this.IsVertical ?
                roads.Where(g => !g.IsVertical && Math.Abs(this.Start.X - g.Start.X) <= 1).ToList() :
                roads.Where(g => g.IsVertical && Math.Abs(this.Start.Y - g.Start.Y) <= 1).ToList();

            return attachedRows;
        }

        public bool AddBuilding(IBuilding building)
        {
            if (building == null || Buildings.Any(b => b.Position.Equals(building.Position))) return false;

            var segment = Segments.First(s => s.Position.X == building.Position.X ||
                                              s.Position.Y == building.Position.Y);

            if (IsVertical)
            {
                building.Direction = building.Position.X > this.Start.X ? Direction.Right : Direction.Left;
            }
            else
            {
                building.Direction = building.Position.Y > this.Start.Y ? Direction.Up : Direction.Down;
            }

            segment.Buildings.Add(building);
            return true;
        }

        public bool RemoveBuilding(IBuilding building)
        {
            if (!this.Buildings.Contains(building)) return false;
            var segment = Segments.First(s => s.Position.X == building.Position.X ||
                                              s.Position.Y == building.Position.Y);
            segment.Buildings.Remove(building);
            return true;
        }

        public RoadType SetUpRoadType(RoadTypeSetUp model)
        {
            if (roadType.Equals(RoadType.Paved)) return roadType;

            double prob = 0;
            var distanceToCenter = model.SettlementCenter.DistanceTo(this.Center);
            if (distanceToCenter < model.AvgDistanceToSettlementCenter)
            {
                prob += 0.2;
            }
            else if (this.Length > 0.5 * MaxRoadLength)
            {
                prob += 0.1;
            }

            foreach (var building in this.Buildings)
            {
                if (building is Residence)
                    prob += 0.01;
                else
                    prob += 0.05;
            }

            switch (model.Epoch)
            {
                case Epoch.First:
                    prob += 0.1;
                    break;
                case Epoch.Second:
                    prob += 0.2;
                    break;
                case Epoch.Third:
                    prob += 0.3;
                    break;
            }

            roadType = RandomProvider.NextDouble() > prob ? RoadType.Unpaved : RoadType.Paved;
            return roadType;
        }

        public override string ToString()
        {
            return $"Road: [{Start};{End}], " +
                   $"Length: {Length}, " +
                   $"buildings: {Segments.SelectMany(s => s.Buildings).Count()}";
        }

        public Road Copy()
        {
            var segments = this.Segments.Select(p => p.Position);
            var copy = new Road(segments);

            this.Buildings.ForEach(b => copy.AddBuilding(((ICopyable<IBuilding>)b).Copy()));
            return copy;
        }

        public class RoadSegment
        {
            public Point Position { get; }
            public List<IBuilding> Buildings { get; }
            public bool IsFull => Buildings.Count == 2;

            public RoadSegment(Point point)
            {
                Position = point;
                Buildings = new List<IBuilding>();
            }
        }
    }
}