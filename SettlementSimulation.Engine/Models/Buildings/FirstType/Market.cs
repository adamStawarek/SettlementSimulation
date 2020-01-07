using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.FirstType
{
    [Epoch(Epoch.First)]
    public class Market : Building
    {
        public override double Probability => 0.01;
        public override bool IsSatisfied(BuildingRule model)
        {
            var minDistanceBetweenMarkets = 50;

            if (!(model.BuildingRoad.Segments.First().Buildings.Contains(this) ||
                model.BuildingRoad.Segments.First().Buildings.Contains(this)))
            {
                //market is at the one tail of the road
                return false;
            }

            var markets = model.Roads.SelectMany(b => b.Buildings).Where(b => b != this && b is Market);
            if (!markets.All(m => m.Position.DistanceTo(this.Position) >= minDistanceBetweenMarkets))
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

            var minDistanceToSettlementCenter =
                model.SettlementUpperLeftBound.DistanceTo(model.SettlementBottomRightBound) / 4;
            if (this.Position.DistanceTo(model.SettlementCenter) > minDistanceToSettlementCenter)
            {
                //market is close enough to the settlement center
                return false;
            }
            return true;

        }
    }
}