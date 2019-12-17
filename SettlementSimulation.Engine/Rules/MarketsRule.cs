using System.Linq;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Rules
{
    /// <summary>
    /// We check whether each markets has empty space around 2 pixel width/height 
    /// </summary>
    public class MarketsRule : IRule
    {
        public bool IsSatisfied(RuleExecutionInfo executionInfo)
        {
            switch (executionInfo.Epoch)
            {
                case Epoch.First:
                {
                    var buildings = executionInfo.Genes.Where(g => g is Building).Cast<Building>().ToArray();
                    var markets = executionInfo.Genes.Where(g => g is Market).Cast<Market>();
                    foreach (var market in markets)
                    {
                        if (buildings.Any(b => b.Location.DistanceTo(market.Location) <= 2))
                        {
                            return false;
                        }

                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (!executionInfo.Fields[market.Location.X + i, market.Location.Y + j].InSettlement)
                                {
                                    return false;
                                }
                            }
                        }
                    }

                    return true;
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