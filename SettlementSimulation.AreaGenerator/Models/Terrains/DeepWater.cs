using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class DeepWater : ITerrain
    {
        public byte UpperBound => 75;
        public Pixel Color => new Pixel(0, 55, 255);
    }
}