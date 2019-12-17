using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using System.Linq;

namespace SettlementSimulation.Engine.Rules
{
    public class SettlementDensityRule : IRule
    {
        private readonly int _maxAverageDistanceBetweenBuildings;

        public SettlementDensityRule(int maxAverageDistanceBetweenBuildings)
        {
            _maxAverageDistanceBetweenBuildings = maxAverageDistanceBetweenBuildings;
        }

        public bool IsSatisfied(RuleExecutionInfo executionInfo)
        {
            switch (executionInfo.Epoch)
            {
                case Epoch.First:
                    {
                        var residences = executionInfo.Genes.Where(g => g is Residence).Cast<Residence>().ToList();
                        var distances = residences.Select(r => residences.Where(g => g != r).Min(g => g.Location.DistanceTo(r.Location)));

                        return distances.Average() < _maxAverageDistanceBetweenBuildings;
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