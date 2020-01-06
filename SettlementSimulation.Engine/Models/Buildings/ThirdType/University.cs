using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.ThirdType
{
    [Epoch(Epoch.Third)]
    public class University : Building
    {
        public override double Probability => 0.005;
        public override bool IsSatisfied(BuildingRule model)
        {
            throw new System.NotImplementedException();
        }
    }
}