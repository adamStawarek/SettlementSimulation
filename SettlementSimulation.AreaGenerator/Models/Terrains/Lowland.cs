using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class Lowland : Terrain
    {
        public override byte UpperBound { get; set; } = 170;
        public override Pixel Color => new Pixel(60, 135, 60);
        public override double Percentile => 0.8;
    }
}