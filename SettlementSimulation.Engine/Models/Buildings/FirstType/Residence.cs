using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.FirstType
{
    [Epoch(Epoch.First)]
    public class Residence : Building
    {
        public override double Probability => 0.885;
        public override int Space => 0;

        public override int CalculateFitness(BuildingRule model)
        {
            return 0;
        }
    }
}