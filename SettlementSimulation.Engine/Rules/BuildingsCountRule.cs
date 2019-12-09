using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using System.Collections.Generic;
using System.Linq;

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

        public bool IsSatisfied(IEnumerable<IStructure> prevBestGenes,
            IEnumerable<IStructure> genes,
            int generation,
            Epoch epoch,
            Field[,] fields)
        {
            switch (epoch)
            {
                case Epoch.First:
                    {
                        if (generation == 1)
                        {
                            var residenceCount = genes.Count(g => g is Residence);
                            return residenceCount >= _minResidences && residenceCount <= _maxResidences;
                        }
                        else
                        {
                            var prevResidenceCount = prevBestGenes.Count(g => g is Residence);
                            var residenceCount = genes.Count(g => g is Residence);
                            return (residenceCount <= 1.1 * prevResidenceCount &&
                                    residenceCount >= 1.05 * prevResidenceCount);
                        }
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