using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IBuildingFitness
    {
        void SetFitness(BuildingRule model);
        double CalculateFitness(BuildingRule model);
        double Fitness { get; set; }
    }
}
