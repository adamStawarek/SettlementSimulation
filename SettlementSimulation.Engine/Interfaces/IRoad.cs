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
       
        //TODO to remove later
        bool IsVertical { get; }
       
        List<Building> Buildings { get; }
        List<Point> AttachedRoads { get; } 
        int Length { get; }

        List<Point> GetPossiblePositionsToAttachRoad(int minDistanceBetweenRoads = 15);
        void AddBuilding(Building building);
        bool BlockCell(Point point);
    }
}