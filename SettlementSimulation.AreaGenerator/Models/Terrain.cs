using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Models
{
    public abstract class Terrain : ITerrain
    {
        public abstract double Percentile { get; }
        public abstract byte UpperBound { get; set; }
        public abstract Pixel Color { get; }

        public void SetHeight(byte height)
        {
            this.UpperBound = height;
        }
    }
}