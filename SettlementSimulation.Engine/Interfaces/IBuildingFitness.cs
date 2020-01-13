using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IBuildingFitness
    {
        int GetFitness(BuildingRule model);
    }
}
