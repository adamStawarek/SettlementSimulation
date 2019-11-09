namespace SettlementSimulation.Engine.Models
{
    public abstract class Building : IStructure
    {
        public Location Location { get; set; }
        public abstract double Probability { get; }
    }
}