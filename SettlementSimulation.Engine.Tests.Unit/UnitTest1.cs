using NUnit.Framework;

namespace GeneticAlgorithm.Tests.Unit
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
            var engine = new BuildingsEngine(100, 10);
            var gene = engine.GetRandomGene();
            Assert.IsInstanceOf<Residence>(gene);
        }

        [Test]
        [Retry(100)]
        public void Test2()
        {
            var engine = new BuildingsEngine(100, 10);
            engine.currentEpoch = Epoch.Second;
            var gene = engine.GetRandomGene();
            Assert.IsInstanceOf<School>(gene);
        }

        [Test]
        [Retry(1000)]
        public void Test3()
        {
            var engine = new BuildingsEngine(100, 10);
            engine.currentEpoch = Epoch.Third;
            var gene = engine.GetRandomGene();
            Assert.IsInstanceOf<University>(gene);
        }
    }
}