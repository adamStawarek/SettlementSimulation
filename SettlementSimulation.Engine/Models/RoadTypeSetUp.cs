using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Enumerators;

namespace SettlementSimulation.Engine.Models
{
    public class RoadTypeSetUp
    {
        public Point SettlementCenter { get; set; }
        public Epoch Epoch { get; set; }
        public int AvgDistanceToSettlementCenter { get; set; }
    }
}