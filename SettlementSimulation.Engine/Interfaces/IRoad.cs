using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IRoad: ISettlementStructure
    {
        List<Road.RoadSegment> Segments { get; }
        Point Start { get; }
        Point End { get; }
        Point Center { get; }
        bool IsVertical { get; }
        List<IBuilding> Buildings { get; }
        int Length { get; }

        List<Point> GetPossiblePositionsToAttachBuilding(List<IRoad> roads);
        List<Point> GetPossiblePositionsToAttachRoad(List<IRoad> roads,int minDistanceBetweenRoads = 15);
        bool AddBuilding(IBuilding building);
    }
}