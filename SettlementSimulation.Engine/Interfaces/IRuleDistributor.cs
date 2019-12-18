namespace SettlementSimulation.Engine.Interfaces
{
    public interface IRuleDistributor
    {
        IRule GetRule<T>(params object[] parameters) where T : IRule;
    }
}