using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Helpers
{
    public interface IRuleDistributor
    {
        IRule GetRule<T>() where T : IRule;
    }
}