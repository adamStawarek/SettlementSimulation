using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models.Buildings;

namespace SettlementSimulation.Engine.Models
{
    public class Road : IRoad
    {
        public Road(IEnumerable<Point> positions)
        {
            Segments = positions.Select(p => new RoadSegment(p)).ToList();
            BlockedCells = new List<Point>();
        }

        public List<RoadSegment> Segments { get; }
        public List<Point> BlockedCells { get; }
        public Point Start => Segments.First().Position;
        public Point End => Segments.Last().Position;
        public int Length => Segments.Count;
        public bool IsVertical => Start.X.Equals(End.X);
        public List<Building> Buildings => Segments.SelectMany(s => s.Buildings).ToList();
        public List<Point> AttachedRoads => BlockedCells
            .Where(c => !Buildings.Select(b => b.Position).Contains(c)).ToList();

        public List<Point> GetPossiblePositionsToAttachBuilding()
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

                points.RemoveAll(p => BlockedCells.Any(b => b.Equals(p)));

                possiblePositions.AddRange(points);
            }

            return possiblePositions;
        }

        public List<Point> GetPossiblePositionsToAttachRoad(int minDistanceBetweenRoads = 15)
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

                points.RemoveAll(p => segment.Buildings.Any(b => b.Position.Equals(p)));

                points.ForEach(p =>
                {
                    if (!AttachedRoads.Any(r => r.DistanceTo(p) < minDistanceBetweenRoads &&
                                              (r.X == p.X || r.Y == p.Y)))
                    {
                        possiblePositions.Add(p);
                    }
                });
            }

            return possiblePositions;
        }

        public void AddBuilding(Building building)
        {
            if (BlockCell(building.Position))
            {
                var segment = Segments.First(s => s.Position.X == building.Position.X ||
                                                  s.Position.Y == building.Position.Y);
                segment.Buildings.Add(building);
            }
        }

        public bool BlockCell(Point point)
        {
            if (BlockedCells.Contains(point) ||
                Segments.All(s => (int)point.DistanceTo(s.Position) > 1))
                return false;

            BlockedCells.Add(point);
            return true;
        }

        public override string ToString()
        {
            return $"Road: [{Start};{End}], " +
                   $"Length: {Length}, " +
                   $"buildings: {Segments.SelectMany(s => s.Buildings).Count()}";
        }

        public class RoadSegment
        {
            public Point Position { get; }
            public List<Building> Buildings { get; }
            public bool IsFull => Buildings.Count == 2;

            public RoadSegment(Point point)
            {
                Position = point;
                Buildings = new List<Building>();
            }
        }
    }
}