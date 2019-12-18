namespace SettlementSimulation.AreaGenerator.Models
{
    public class Field
    {
        public Point Position { get; set; }
        public bool InSettlement { get; set; }
        public double DistanceToWater { get; set; }
        public double DistanceToMainRoad { get; set; }
    }
}