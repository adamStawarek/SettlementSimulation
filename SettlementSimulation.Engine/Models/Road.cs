using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models.Buildings;

namespace SettlementSimulation.Engine.Models
{
    public class Road : IRoad, ICopyable<Road>
    {
        public Road(IEnumerable<Point> positions)
        {
            Segments = positions.Select(p => new RoadSegment(p)).ToList();
        }

        public List<RoadSegment> Segments { get; }
        public Point Start => Segments.First().Position;
        public Point End => Segments.Last().Position;
        public Point Center => new Point((Start.X + End.X) / 2, (Start.Y + End.Y) / 2);
        public int Length => Segments.Count;
        public bool IsVertical => Start.X.Equals(End.X);
        public List<IBuilding> Buildings => Segments.SelectMany(s => s.Buildings).ToList();

        public List<Point> GetPossiblePositionsToAttachBuilding(List<IRoad> roads)
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

                points.RemoveAll(p => Buildings.Select(b=>b.Position).Any(b => b.Equals(p)) ||
                                      roads.Any(r => r.Start.Equals(p) || r.End.Equals(p)));

                possiblePositions.AddRange(points);
            }

            return possiblePositions;
        }

        public List<Point> GetPossiblePositionsToAttachRoad(List<IRoad> roads, int minDistanceBetweenRoads = 15)
        {
            var possiblePositions = new List<Point>();
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
                        points.RemoveAll(p => (r.Start.DistanceTo(p) < minDistanceBetweenRoads && (r.Start.X == p.X) ||
                                               (r.End.DistanceTo(p) < minDistanceBetweenRoads && (r.End.X == p.X))));
                    }
                    else if (r.IsVertical && !this.IsVertical)
                    {
                        points.RemoveAll(p => (r.Start.DistanceTo(p) < minDistanceBetweenRoads && (r.Start.Y == p.Y) ||
                                               (r.End.DistanceTo(p) < minDistanceBetweenRoads && (r.End.Y == p.Y))));
                    }
                }

                points.ForEach(p =>
                {
                    possiblePositions.Add(p);
                });
            }

            if (this.IsVertical)
            {
                possiblePositions.RemoveAll(
                    p => roads.Where(g => !g.IsVertical).Any(g => Math.Abs(g.Start.Y - p.Y) <= 2));

            }
            else
            {
                possiblePositions.RemoveAll(
                    p => roads.Where(g => g.IsVertical).Any(g => Math.Abs(g.Start.X - p.X) <= 2));
            }

            return possiblePositions;
        }

        public void AddBuilding(IBuilding building)
        {
            if (building == null || Buildings.Any(b=>b.Position.Equals(building.Position))) return;

            var segment = Segments.First(s => s.Position.X == building.Position.X ||
                                              s.Position.Y == building.Position.Y);
            segment.Buildings.Add(building);
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