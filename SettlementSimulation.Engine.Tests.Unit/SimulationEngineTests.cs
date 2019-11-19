using NUnit.Framework;
using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class SimulationEngineTests
    {
        private SimulationEngine _engine;
        
        [SetUp]
        public void Setup()
        {
            _engine = new SimulationEngine(100, 10);
        }

        [Test]
        [Retry(10)]
        public void At_First_Epoch_GetRandomGene_Generates_At_Least_One_Residence_In_10_Tries()
        {
            var gene = _engine.GetRandomGene();
            Assert.IsInstanceOf<Residence>(gene);
        }

        [Test]
        [Retry(100)]
        public void At_Second_Epoch_GetRandomGene_Generates_At_Least_One_School_In_100_Tries()
        {
            _engine.SetNextEpoch();
            var gene = _engine.GetRandomGene();
            Assert.IsInstanceOf<School>(gene);
        }

        [Test]
        [Retry(1000)]
        public void At_Third_Epoch_GetRandomGene_Generates_At_Least_One_University_In_1000_Tries()
        {
            _engine.SetNextEpoch();
            _engine.SetNextEpoch();
            var gene = _engine.GetRandomGene();
            Assert.IsInstanceOf<University>(gene);
        }
    }
}