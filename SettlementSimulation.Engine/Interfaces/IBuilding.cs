using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IBuilding:ISettlementStructure
    {
        double Probability { get; }
        Point Position { get; set; }
    }
}