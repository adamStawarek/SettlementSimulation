using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IRule
    {
        bool IsSatisfied(RuleExecutionInfo executionInfo);
    }
}
