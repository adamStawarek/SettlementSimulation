using System.Linq;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Rules
{
    /// <summary>
    /// We check whether there is minimum # of buildings of given type
    /// </summary>
    public class BuildingsCountRule : IRule
    {
        public bool IsSatisfied(RuleExecutionInfo executionInfo)
        {
            switch (executionInfo.Epoch)
            {
                case Epoch.First:
                    {
                        var residences = executionInfo.Genes.Count(g => g is Residence);
                        var markets = executionInfo.Genes.Count(g => g is Market);
                        return residences >= 50 & markets >= 1 && markets <= 3;
                    }
                case Epoch.Second:
                    {
                        break;
                    }
                case Epoch.Third:
                    {
                        break;
                    }
            }

            return false;
        }
    }
}