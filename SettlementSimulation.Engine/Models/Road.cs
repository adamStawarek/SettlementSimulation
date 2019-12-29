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
            AttachedRoads = new Dictionary<Point, Road>();
            Segments = positions.Select(p=>new RoadSegment(p)).ToList();
        }

        public List<RoadSegment> Segments { get; }
        public Dictionary<Point, Road> AttachedRoads { get; }
        public Point Start => Segments.First().Position;
        public Point End => Segments.Last().Position;

        public override string ToString()
        {
            return $"Road: [{Start};{End}], buildings: {Segments.SelectMany(s=>s.Buildings).Count()}";
        }

        public class RoadSegment
        {
            public Point Position { get; }
            public List<Building> Buildings { get; }

            public RoadSegment(Point point)
            {
                Position = point;
                Buildings = new List<Building>();
            }
        }
    }
}