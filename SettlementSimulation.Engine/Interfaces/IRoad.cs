using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IRoad
    {
        List<Road.RoadSegment> Segments { get; }
        List<Point> BlockedCells { get; }
        Point Start { get; }
        Point End { get; }
        Point Center { get; }
        bool IsVertical { get; } //TODO to remove later
        List<Building> Buildings { get; }
        int Length { get; }

        List<Point> GetPossiblePositionsToAttachBuilding(List<IRoad> roads);
        List<Point> GetPossiblePositionsToAttachRoad(List<IRoad> roads,int minDistanceBetweenRoads = 15);
        void AddBuilding(Building building);
    }
}