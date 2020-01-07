using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.FirstType
{
    [Epoch(Epoch.First)]
    public class Tavern : Building
    {
        public override double Probability => 0.01;
        public override bool IsSatisfied(BuildingRule model)
        {
            var minDistanceBetweenTaverns = 5;
            var taverns = model.Roads.SelectMany(b => b.Buildings).Where(b => b != this && b is Tavern);
            if (!taverns.All(m => m.Position.DistanceTo(this.Position) >= minDistanceBetweenTaverns))
            {
                //all other markets are far enough
                return false;
            }

            var residences = model.Roads.SelectMany(b => b.Buildings).Where(b => b is Residence);
            if (residences.Count(r => r.Position.DistanceTo(this.Position) <= 10) <= 100)
            {
                //there are enough residences in closest neighborhood
                return false;
            }

            return true;
        }
    }
}