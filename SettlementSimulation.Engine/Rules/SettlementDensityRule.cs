using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using System.Collections.Generic;
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

        public bool IsSatisfied(IEnumerable<IStructure> prevBestGenes,
            IEnumerable<IStructure> genes,
            int generation,
            Epoch epoch,
            IEnumerable<Field> settlementFields)
        {
            switch (epoch)
            {
                case Epoch.First:
                    {
                        var residences = genes.Where(g => g is Residence).Cast<Residence>().ToList();
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