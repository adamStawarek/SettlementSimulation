using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Models.Buildings.SecondType;

namespace SettlementSimulation.Engine.Models.Buildings.ThirdType
{
    [Epoch(Epoch.Third)]
    public class University : Building
    {
        public override double Probability => 0.005;
        public override bool IsSatisfied(BuildingRule model)
        {
            var schoolsCount = model.Roads.SelectMany(b => b.Buildings).Count(b => b != this && b is School);
            if (schoolsCount < 3)
            {
                // there are at least 3 schools
                return false;
            }

            var universitiesCount = model.Roads.SelectMany(b => b.Buildings).Count(b => b != this && b is University);
            if (universitiesCount < schoolsCount / 5)
            {
                //there area at least 5 schools per university
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