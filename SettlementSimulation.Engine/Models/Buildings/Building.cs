using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Models.Buildings
{
    public abstract class Building : IStructure
    {
        public Location Location { get; set; }
        public abstract double Probability { get; }
    }
}