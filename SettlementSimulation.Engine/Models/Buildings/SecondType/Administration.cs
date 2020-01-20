using System;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Models.Buildings.SecondType
{
    [Epoch(Epoch.Second)]
    public class Administration : Building
    {
        public override double Probability => 0.001;
        public override int Space => 1;

        public override int CalculateFitness(BuildingRule model)
        {
            var maxDistanceToCenter = 10;
            if (Position.DistanceTo(model.SettlementCenter) > maxDistanceToCenter)
            {
                //market is close enough to the settlement center
                return 0;
            }

            var buildings = model.Roads.SelectMany(b => b.Buildings).Count();
            var administrations = model.Roads.SelectMany(b => b.Buildings).Count(b => b is Administration);
            if (buildings / (administrations + 1) < 100)
            {
                Console.WriteLine("No more than one administration per 1000 buildings");
                return 0;
            }

            return 10;
        }
    }
}