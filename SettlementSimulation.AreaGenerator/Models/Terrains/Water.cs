using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class Water : Terrain
    {
        public override double Percentile => 0.15;
        public override byte UpperBound { get; set; } = 130;
        public override Pixel Color => new Pixel(0, 115, 255);
    }
}