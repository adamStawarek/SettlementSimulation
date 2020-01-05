using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class MountainBottom : ITerrain
    {
        public byte UpperBound => 230;
        public Pixel Color => new Pixel(115, 65, 45);
        public double Percentile => 0.95;
    }
}