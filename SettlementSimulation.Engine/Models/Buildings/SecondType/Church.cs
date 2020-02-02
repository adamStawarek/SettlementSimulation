using System;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Models.Buildings.SecondType
{
    [Epoch(Epoch.Second)]
    public class Church : Building
    {
        public override double Probability => 0.02;
        public override int Space => 1;

        public override double CalculateFitness(BuildingRule model)
        {
            var minDistanceBetweenChurches = 30;
            var churches = model.Roads.SelectMany(b => b.Buildings).Where(b => b != this && b is Church);
            if (!churches.All(m => m.Position.DistanceTo(this.Position) >= minDistanceBetweenChurches))
            {
                return 0;
            }

            var residences = model.Roads.SelectMany(b => b.Buildings).Where(b => b is Residence);
            if (residences.Count(r => r.Position.DistanceTo(this.Position) <= 30) < 500)
            {
                return 0;
            }
            return 5;
        }
    }
}