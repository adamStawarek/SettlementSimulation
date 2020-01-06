using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class MountainTop : Terrain
    {
        public override byte UpperBound { get; set; } = byte.MaxValue;
        public override Pixel Color => new Pixel(255, 255, 255);
        public override double Percentile => 1;
    }
}