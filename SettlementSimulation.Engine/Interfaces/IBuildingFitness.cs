using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IBuildingFitness
    {
        void SetFitness(BuildingRule model);
        int CalculateFitness(BuildingRule model);
        int Fitness { get; set; }
    }
}
