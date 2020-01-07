using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.SecondType
{
    [Epoch(Epoch.Second)]
    public class Church : Building
    {
        public override double Probability => 0.03;
        public override bool IsSatisfied(BuildingRule model)
        {
            var minDistanceBetweenChurches = 15;
            var churches = model.Roads.SelectMany(b => b.Buildings).Where(b => b != this && b is Church);
            if (!churches.All(m => m.Position.DistanceTo(this.Position) >= minDistanceBetweenChurches))
            {
                //all other markets are far enough
                return false;
            }

            return true;
        }
    }
}