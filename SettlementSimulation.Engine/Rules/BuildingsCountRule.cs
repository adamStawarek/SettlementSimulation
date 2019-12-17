using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Rules
{
    public class BuildingsCountRule : IRule
    {
        private readonly int _minResidences;
        private readonly int _maxResidences;

        public BuildingsCountRule(int minResidences = 10, int maxResidences = 50)
        {
            this._minResidences = maxResidences;
            this._maxResidences = maxResidences;
        }

        public bool IsSatisfied(RuleExecutionInfo executionInfo)
        {
            switch (executionInfo.Epoch)
            {
                case Epoch.First:
                    {
                        break;
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