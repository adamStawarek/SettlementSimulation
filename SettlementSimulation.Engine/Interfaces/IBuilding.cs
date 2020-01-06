using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IBuilding : ISettlementStructure, IBuildingRule
    {
        double Probability { get; }
        Point Position { get; set; }
    }
}