using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class HighGround : Terrain
    {
        public override byte UpperBound { get; set; } = 200;
        public override Pixel Color => new Pixel(140, 140, 70);
        public override double Percentile => 0.85;
    }
}