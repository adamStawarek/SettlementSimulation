using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IBuilding
    {
        double Probability { get; }
        Point Position { get; set; }
    }
}