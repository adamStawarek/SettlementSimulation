using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models.Buildings.SecondType
{
    [Epoch(Epoch.Second)]
    public class AdministrationOffice : Building
    {
        public override double Probability => 0.03;
    }
}