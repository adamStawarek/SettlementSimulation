using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.ThirdType
{
    [Epoch(Epoch.Third)]
    public class Court : Building
    {
        public override double Probability => 0.005;
    }
}