using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class HighGround : ITerrain
    {
        public byte UpperBound => 200;
        public Pixel Color => new Pixel(140, 140, 70);
        public double Percentile => 0.85;
    }
}