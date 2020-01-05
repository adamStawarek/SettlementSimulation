using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class DeepWater : ITerrain
    {
        public double Percentile => 0.05;
        public byte UpperBound => 75;
        public Pixel Color => new Pixel(0, 55, 255);
    }
}