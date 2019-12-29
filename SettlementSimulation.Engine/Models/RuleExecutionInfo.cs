using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using System.Collections.Generic;

namespace SettlementSimulation.Engine.Models
{
    public class RuleExecutionInfo
    {
        public IEnumerable<IBuilding> Genes { get; set; }
        public Field[,] Fields { get; set; }
        public int Generation { get; set; }
        public Epoch Epoch { get; set; }
    }
}