using SettlementSimulation.Engine.Interfaces;
using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.Engine.Models
{
    public class SettlementState
    {
        public List<IRoad> Roads { get; set; }
        public Point SettlementCenter { get; set; }
        public IEnumerable<ISettlementStructure> LastCreatedStructures { get; set; }
        public Epoch CurrentEpoch { get; set; }
        public int CurrentGeneration { get; set; }
        public int Time { get; set; }
    }
}