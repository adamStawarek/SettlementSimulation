using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Rules
{
    public class DistanceToWaterRule : IRule
    {
        private readonly int _maxDistanceToWater;

        public DistanceToWaterRule(int maxDistanceToWater)
        {
            this._maxDistanceToWater = maxDistanceToWater;
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
                        var residencesLocations = genes.Where(g => g is Residence).Cast<Residence>().Select(r => r.Location).ToList();
                        var fields = settlementFields.Where(f => residencesLocations.Contains(f.Location)).ToList();
                        var closestToWater = fields.OrderByDescending(f => f.DistanceToWater).Take((100));
                        return closestToWater.Average(f => f.DistanceToWater) < _maxDistanceToWater;
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