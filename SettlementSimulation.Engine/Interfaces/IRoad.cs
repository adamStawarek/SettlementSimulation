using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Models;
namespace SettlementSimulation.Engine.Interfaces
{
    public interface IRoad : ISettlementStructure, ICopyable<Road>
    {
        List<Road.RoadSegment> Segments { get; }
        Point Start { get; }
        Point End { get; }
        Point Center { get; }
        bool IsVertical { get; }
        List<IBuilding> Buildings { get; }
        int Length { get; }
        RoadType Type { get; }

        List<Point> GetPossiblePositionsToAttachBuilding(List<IRoad> roads);
        List<Point> GetPossiblePositionsToAttachRoad(List<IRoad> roads, int minDistanceBetweenRoads = 15);
        List<IRoad> AttachedRoads(List<IRoad> roads);
        bool AddBuilding(IBuilding building);
    }
}