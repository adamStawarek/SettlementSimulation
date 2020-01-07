using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Models.Buildings.SecondType
{
    [Epoch(Epoch.Second)]
    public class School : Building
    {
        public override double Probability => 0.005;
        public override bool IsSatisfied(BuildingRule model)
        {
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