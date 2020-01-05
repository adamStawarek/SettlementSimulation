using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class MountainTop : ITerrain
    {
        public byte UpperBound => byte.MaxValue;
        public Pixel Color => new Pixel(255, 255, 255);
        public double Percentile => 1;
    }
}