using NUnit.Framework;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Rules;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class RuleDistributorTests
    {
        [Test]
        public void GetRule_Returns_Instance_Of_Rule()
        {
            var ruleDistributor = new RuleDistributor();
            var rule = ruleDistributor.GetRule<BuildingsCountRule>(10, 20);
            Assert.IsInstanceOf<BuildingsCountRule>(rule);
        }
    }
}
