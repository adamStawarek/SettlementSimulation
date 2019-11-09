using NUnit.Framework;
using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Retry(10)]
        public void Test1()
        {
            var engine = new SimulationEngine(100, 10);
            var gene = engine.GetRandomGene();
            Assert.IsInstanceOf<Residence>(gene);
        }

        [Test]
        [Retry(100)]
        public void Test2()
        {
            var engine = new SimulationEngine(100, 10);
            engine.SetNextEpoch();
            var gene = engine.GetRandomGene();
            Assert.IsInstanceOf<School>(gene);
        }

        [Test]
        [Retry(1000)]
        public void Test3()
        {
            var engine = new SimulationEngine(100, 10);
            engine.SetNextEpoch();
            engine.SetNextEpoch();
            var gene = engine.GetRandomGene();
            Assert.IsInstanceOf<University>(gene);
        }
    }
}