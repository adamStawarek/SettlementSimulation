using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class Water : ITerrain
    {
        public double Percentile => 0.15;
        public byte UpperBound => 130;
        public Pixel Color => new Pixel(0, 115, 255);
    }
}