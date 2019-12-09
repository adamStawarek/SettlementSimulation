using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using System.Collections.Generic;

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
            Field[,] fields)
        {
            switch (epoch)
            {
                case Epoch.First:
                    {
                        //var residencesLocations = genes.Where(g => g is Residence).Cast<Residence>().Select(r => r.Location).ToList();
                        //var fields = fields.Where(f => residencesLocations.Contains(f.Location)).ToList();
                        //var closestToWater = fields.OrderByDescending(f => f.DistanceToWater).Take((100));
                        //return closestToWater.Average(f => f.DistanceToWater) <= _maxDistanceToWater;
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