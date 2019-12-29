using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.AreaGenerator.Interfaces
{
    public interface ITerrain
    {
        byte UpperBound { get; }

        Pixel Color { get; }
    }
}