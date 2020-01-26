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
        int Age { get; set; }

        List<Point> GetPossibleBuildingPositions(PossibleBuildingPositions model);
        List<Point> GetPossibleRoadPositions(PossibleRoadPositions model);
        List<IRoad> AttachedRoads(List<IRoad> roads);
        bool AddBuilding(IBuilding building);
        bool RemoveBuilding(IBuilding building);
        bool IsCrossed(IRoad other);
        RoadType SetUpRoadType(RoadTypeSetUp model);
        void SetRoadType(RoadType roadType);
    }
}