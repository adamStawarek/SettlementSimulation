using System;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.FirstType
{
    [Epoch(Epoch.First)]
    public class Market : Building
    {
        public override double Probability => 0.005;
        public override int Space => 2;

        public override int CalculateFitness(BuildingRule model)
        {
            var minDistanceBetweenMarkets = 50;
            var markets = model.Roads.SelectMany(b => b.Buildings).Where(b => b != this && b is Market);
            if (!markets.All(m => m.Position.DistanceTo(this.Position) >= minDistanceBetweenMarkets))
            {
                Console.WriteLine("Other markets are too close");
                return 0;
            }

            var residences = model.Roads.SelectMany(b => b.Buildings).Where(b => b is Residence);
            if (residences.Count(r => r.Position.DistanceTo(this.Position) <= 50) < 100)
            {
                Console.WriteLine("Not enough residences in closest neighborhood");
                return 0;
            }

            return 5;
        }
    }
}