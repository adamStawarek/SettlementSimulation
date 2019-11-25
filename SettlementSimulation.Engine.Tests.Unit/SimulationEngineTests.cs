using NUnit.Framework;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using SettlementSimulation.Engine.Models.Buildings.SecondType;
using SettlementSimulation.Engine.Models.Buildings.ThirdType;
using SettlementSimulation.Engine.Rules;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class SimulationEngineTests
    {
        private SimulationEngine _engine;

        [SetUp]
        public void Setup()
        {
            var fields = new[]
            {
                new Field(0,0){DistanceToWater = 10},
                new Field(1,0){DistanceToWater = 10},
                new Field(2,0){DistanceToWater = 10},
                new Field(3,0){DistanceToWater = 10},

                new Field(0,1){DistanceToWater = 20},
                new Field(1,1){DistanceToWater = 20},
                new Field(2,1){DistanceToWater = 20},
                new Field(3,1){DistanceToWater = 20},

                new Field(0,2){DistanceToWater = 30},
                new Field(1,2){DistanceToWater = 30},
                new Field(2,2){DistanceToWater = 30},
                new Field(3,2){DistanceToWater = 30},

                new Field(0,3){DistanceToWater = 40},
                new Field(1,3){DistanceToWater = 40},
                new Field(2,3){DistanceToWater = 40},
                new Field(3,3){DistanceToWater = 40},

                new Field(0,4){DistanceToWater = 50},
                new Field(1,4){DistanceToWater = 50},
                new Field(2,4){DistanceToWater = 50},
                new Field(3,4){DistanceToWater = 50}
            };
            _engine = new SimulationEngine(10, 10, fields);
            _engine.AddRules(new IRule[]
            {
                new BuildingsCountRule(1,2),
                new SettlementDensityRule(3),
                new DistanceToWaterRule(10)
            });
            _engine.Population[0].Genes = new IStructure[]
            {
                new EmptyArea() {Location = new Location(0, 0)},
                new EmptyArea() {Location = new Location(1, 0)},
                new EmptyArea() {Location = new Location(2, 0)},
                new EmptyArea() {Location = new Location(3, 0)},
                new EmptyArea() {Location = new Location(0, 1)},
                new EmptyArea() {Location = new Location(1, 1)},
                new EmptyArea() {Location = new Location(2, 1)},
                new EmptyArea() {Location = new Location(3, 1)},
                new EmptyArea() {Location = new Location(0, 2)},
                new EmptyArea() {Location = new Location(1, 2)}
            };
            _engine.Population[1].Genes = new IStructure[]
            {
                new Residence() {Location = new Location(0, 0)},
                new EmptyArea() {Location = new Location(1, 0)},
                new EmptyArea() {Location = new Location(2, 0)},
                new EmptyArea() {Location = new Location(3, 0)},
                new EmptyArea() {Location = new Location(0, 1)},
                new EmptyArea() {Location = new Location(1, 1)},
                new EmptyArea() {Location = new Location(2, 1)},
                new EmptyArea() {Location = new Location(3, 1)},
                new EmptyArea() {Location = new Location(0, 2)},
                new Residence() {Location = new Location(0, 4)}
            };
            _engine.Population[2].Genes = new IStructure[]
            {
                new Residence() {Location = new Location(0, 0)},
                new EmptyArea() {Location = new Location(1, 0)},
                new EmptyArea() {Location = new Location(2, 0)},
                new Residence() {Location = new Location(3, 0)},
                new EmptyArea() {Location = new Location(0, 1)},
                new EmptyArea() {Location = new Location(1, 1)},
                new EmptyArea() {Location = new Location(2, 1)},
                new EmptyArea() {Location = new Location(3, 1)},
                new EmptyArea() {Location = new Location(0, 2)},
                new EmptyArea() {Location = new Location(0, 4)}
            };
            _engine.Population[3].Genes = new IStructure[]
            {
                new EmptyArea() {Location = new Location(0, 0)},
                new EmptyArea() {Location = new Location(1, 0)},
                new EmptyArea() {Location = new Location(2, 0)},
                new EmptyArea() {Location = new Location(3, 0)},
                new EmptyArea() {Location = new Location(0, 1)},
                new EmptyArea() {Location = new Location(1, 1)},
                new EmptyArea() {Location = new Location(2, 1)},
                new EmptyArea() {Location = new Location(3, 1)},
                new Residence() {Location = new Location(0, 4)},
                new Residence() {Location = new Location(1, 4)}
            };
            _engine.Population[4].Genes = new IStructure[]
            {
                new Residence() {Location = new Location(0, 0)},
                new Residence() {Location = new Location(1, 0)},
                new EmptyArea() {Location = new Location(2, 0)},
                new EmptyArea() {Location = new Location(3, 0)},
                new EmptyArea() {Location = new Location(0, 1)},
                new EmptyArea() {Location = new Location(1, 1)},
                new EmptyArea() {Location = new Location(2, 1)},
                new EmptyArea() {Location = new Location(3, 1)},
                new EmptyArea() {Location = new Location(0, 2)},
                new EmptyArea() {Location = new Location(0, 4)}
            };
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

        [Test]
        public void At_First_Generation_When_Subject_Does_Not_Satisfy_BuildingsCountRule__SubjectFitness_Returns_0()
        {
            Assert.AreEqual(0, _engine.SubjectFitness(0), 0.01);
        }

        [Test]
        public void At_First_Generation_When_Subject_Satisfy_Only_BuildingsCountRule__SubjectFitness_Returns_1()
        {
            Assert.AreEqual(1, _engine.SubjectFitness(1), 0.01);
        }

        [Test]
        public void At_First_Generation_When_Subject_Satisfy_BuildingsCountRule_And_DistanceToWaterRule__SubjectFitness_Returns_2()
        {
            Assert.AreEqual(2, _engine.SubjectFitness(2), 0.01);
        }

        [Test]
        public void At_First_Generation_When_Subject_Satisfy_BuildingsCountRule_And_SettlementDensityRule__SubjectFitness_Returns_2()
        {
            Assert.AreEqual(2, _engine.SubjectFitness(3), 0.01);
        }

        [Test]
        public void At_First_Generation_When_Subject_Satisfy_BuildingsCountRule_SettlementDensityRule_And_DistanceToWaterRule_SubjectFitness_Returns_3()
        {
            Assert.AreEqual(3, _engine.SubjectFitness(4), 0.01);
        }
    }
}