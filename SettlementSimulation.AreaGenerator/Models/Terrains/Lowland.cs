using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class Lowland : ITerrain
    {
        public byte UpperBound => 170;
        public Pixel Color => new Pixel(60, 135, 60);
        public float Coverage => 0.5f;
    }
}