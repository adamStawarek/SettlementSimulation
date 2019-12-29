using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IRoad
    {
        List<Road.RoadSegment> Segments { get; }
        Dictionary<Point, Road> AttachedRoads { get; }
        Point Start { get; }
        Point End { get; }
    }
}