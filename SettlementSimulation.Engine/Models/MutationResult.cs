using System;
using System.Collections.Generic;
using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Models
{
    public class MutationResult
    {
        public MutationResult()
        {
            RemovedBuildings = new List<IBuilding>();
        }

        public List<IBuilding> RemovedBuildings { get; set; }
    }
}
