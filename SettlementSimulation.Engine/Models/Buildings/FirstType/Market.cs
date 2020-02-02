using System;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.FirstType
{
    [Epoch(Epoch.First)]
    public class Market : Building
    {
        public override double Probability => 0.05;
        public override int Space => 2;

        public override double CalculateFitness(BuildingRule model)
        {
            var minDistanceBetweenMarkets = 30;
            var markets = model.Roads.SelectMany(b => b.Buildings).Where(b => b != this && b is Market);
            if (!markets.All(m => m.Position.DistanceTo(this.Position) >= minDistanceBetweenMarkets))
            {
                //("Other markets are too close");
                return 0;
            }

            var residences = model.Roads.SelectMany(b => b.Buildings).Where(b => b is Residence);
            if (residences.Count(r => r.Position.DistanceTo(this.Position) <= 20) < 250)
            {
                //("Not enough residences in closest neighborhood");
                return 0;
            }

            return 5;
        }
    }
}