using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Enumerators;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IBuilding : ISettlementStructure, IBuildingFitness
    {
        int Space { get; }
        double Probability { get; }
        Point Position { get; set; }
        Direction? Direction { get; set; }       
    }
}