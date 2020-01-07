using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.SecondType
{
    [Epoch(Epoch.Second)]
    public class Administration : Building
    {
        public override double Probability => 0.001;
        public override bool IsSatisfied(BuildingRule model)
        {
            var minDistanceToSettlementCenter = 15;
            if (this.Position.DistanceTo(model.SettlementCenter) <= minDistanceToSettlementCenter)
            {
                //market is close enough to the settlement center
                return false;
            }
            return true;
        }
    }
}