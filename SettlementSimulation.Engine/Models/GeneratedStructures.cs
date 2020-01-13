using System.Collections.Generic;
using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Models
{
    public class GeneratedStructures
    {
        public GeneratedStructures()
        {
            NewRoads = new List<IRoad>();
            NewBuildings = new List<IBuilding>();
        }

        public List<IRoad> NewRoads { get; set; }
        public List<IBuilding> NewBuildings { get; set; }
        public IRoad RoadToAttachNewBuildings { get; set; }
    }
}