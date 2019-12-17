using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Helpers
{
    public interface IRuleDistributor
    {
        IRule GetRule<T>(params object[] parameters) where T : IRule;
    }
}