using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class DeepWater : Terrain
    {
        public override double Percentile => 0.05;
        public override byte UpperBound { get; set; } = 75;
        public override Pixel Color => new Pixel(0, 55, 255);
    }
}