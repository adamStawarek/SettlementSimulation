using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Models
{
    public class BuildingRule
    {
        public Field[,] Fields { get; set; }
        public List<IRoad> Roads { get; set; }
        public IRoad BuildingRoad { get; set; }
        public Point SettlementCenter { get; set; }
        public Point SettlementUpperLeftBound { get; set; }
        public Point SettlementBottomRightBound { get; set; }
    }
}