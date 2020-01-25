using System;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Models.Buildings.SecondType
{
    [Epoch(Epoch.Second)]
    public class School : Building
    {
        public override double Probability => 0.02;
        public override int Space => 2;

        public override double CalculateFitness(BuildingRule model)
        {
            var residences = model.Roads.SelectMany(b => b.Buildings).Count(b => b is Residence);
            var schools = model.Roads.SelectMany(b => b.Buildings).Count(b => b is School);
            if (residences / (schools + 1) < 500)
            {
                Console.WriteLine("No more than one school per 300 residences");
                return 0;
            }

            return 5;
        }
    }
}