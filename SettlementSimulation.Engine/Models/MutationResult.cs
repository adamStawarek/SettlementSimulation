using System;
using System.Collections.Generic;
using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Models
{
    public class MutationResult
    {
        public MutationResult()
        {
            RemovedRoads = new List<IRoad>();
            RemovedBuildings = new List<IBuilding>();
        }

        public List<IRoad> RemovedRoads { get; set; }
        public List<IBuilding> RemovedBuildings { get; set; }
    }
}
