using NUnit.Framework;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class BuildingRulesTests
    {
        [Test,Ignore("In development")]
        public void When_Rules_For_Market_Are_Satisfied()
        {
            var market = new Market()
            {
                Position =new Point()
            };

        }

        [Test, Ignore("In development")]
        public void When_Rules_For_Market_Are_Not_Satisfied()
        {
            var market = new Market()
            {
                Position = new Point()
            };
        }
    }
}