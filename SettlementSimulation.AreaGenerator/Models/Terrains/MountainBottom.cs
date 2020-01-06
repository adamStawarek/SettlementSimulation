using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class MountainBottom : Terrain
    {
        public override byte UpperBound { get; set; } = 230;
        public override Pixel Color => new Pixel(115, 65, 45);
        public override double Percentile => 0.95;
    }
}