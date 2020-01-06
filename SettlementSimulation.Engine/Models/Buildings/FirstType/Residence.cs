using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.FirstType
{
    [Epoch(Epoch.First)]
    public class Residence : Building
    {
        public override double Probability => 0.7;
        public override bool IsSatisfied(BuildingRule model)
        {
            return false;
        }
    }
}