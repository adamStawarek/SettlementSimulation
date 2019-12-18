using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models.Terrains
{
    public class Sand : ITerrain
    {
        public byte UpperBound => 145;
        public Pixel Color => new Pixel(215, 215, 115);
    }
}