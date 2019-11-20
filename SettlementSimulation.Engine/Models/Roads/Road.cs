using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Models.Roads
{
    public abstract class Road : IStructure
    {
        public abstract double Probability { get; }
    }
}