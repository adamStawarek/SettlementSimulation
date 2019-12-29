using SettlementSimulation.Engine.Interfaces;
using System.Collections.Generic;

namespace SettlementSimulation.Engine.Models
{
    public class SettlementState
    {
        public List<IRoad> Structures { get; set; }
        public Epoch CurrentEpoch { get; set; }
        public int CurrentGeneration { get; set; }
        public int Time { get; set; }
    }
}