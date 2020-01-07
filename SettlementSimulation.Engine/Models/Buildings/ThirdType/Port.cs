using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.ThirdType
{
    [Epoch(Epoch.Third)]
    public class Port : Building
    {
        public override double Probability => 0.005;
        public override bool IsSatisfied(BuildingRule model)
        {
            var maxPortDistanceToWater = 10;
            var field = model.Fields[this.Position.X, this.Position.Y];
            if (field.DistanceToWater > maxPortDistanceToWater)
            {
                return false;
            }

            return true;
        }
    }
}