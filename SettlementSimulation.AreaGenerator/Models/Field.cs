using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models
{
    public class Field
    {
        public Point Position { get; set; }
        public ITerrain Terrain { get; set; }
        public bool InSettlement { get; set; }
        public bool? IsBlocked { get; set; }
        public double DistanceToWater { get; set; }
        public double DistanceToMainRoad { get; set; }
    }
}