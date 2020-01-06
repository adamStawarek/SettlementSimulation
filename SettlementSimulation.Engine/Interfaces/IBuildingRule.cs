using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IBuildingRule
    {
        bool IsSatisfied(BuildingRule model);
    }
}
