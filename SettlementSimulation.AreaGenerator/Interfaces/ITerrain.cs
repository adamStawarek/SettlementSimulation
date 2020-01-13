using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.AreaGenerator.Interfaces
{
    public interface ITerrain
    {
        double Percentile { get; }
        byte UpperBound { get; set; }

        Pixel Color { get; }
        void SetHeight(byte height);
    }
}