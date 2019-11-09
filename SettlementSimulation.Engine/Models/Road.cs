namespace SettlementSimulation.Engine.Models
{
    public abstract class Road : IStructure
    {
        public abstract double Probability { get; }
    }
}