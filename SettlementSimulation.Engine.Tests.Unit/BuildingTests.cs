using NUnit.Framework;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using SettlementSimulation.Engine.Models.Buildings.SecondType;
using SettlementSimulation.Engine.Models.Buildings.ThirdType;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class BuildingTests
    {
        [Test]
        [Retry(10)]
        public void At_First_Epoch_GetRandomGene_Generates_At_Least_One_Residence_In_10_Tries()
        {
            var gene = Building.GetRandom(Epoch.First);
            Assert.IsInstanceOf<Residence>(gene);
        }

        [Test]
        [Retry(1000)]
        public void At_First_Epoch_GetRandomGene_Generates_At_Least_One_Tavern_In_1000_Tries()
        {
            var gene = Building.GetRandom(Epoch.First);
            Assert.IsInstanceOf<Tavern>(gene);
        }

        [Test]
        [Retry(1000)]
        public void At_Second_Epoch_GetRandomGene_Generates_At_Least_One_School_In_1000_Tries()
        {
            var gene = Building.GetRandom(Epoch.Second);
            Assert.IsInstanceOf<School>(gene);
        }

        [Test]
        [Retry(1000)]
        public void At_Second_Epoch_GetRandomGene_Generates_At_Least_One_Church_In_1000_Tries()
        {
            var gene = Building.GetRandom(Epoch.Second);
            Assert.IsInstanceOf<Church>(gene);
        }

        [Test]
        [Retry(2000)]
        public void At_Second_Epoch_GetRandomGene_Generates_At_Least_One_AdministrationOffice_In_2000_Tries()
        {
            var gene = Building.GetRandom(Epoch.Second);
            Assert.IsInstanceOf<Administration>(gene);
        }

        [Test]
        [Retry(1000)]
        public void At_Third_Epoch_GetRandomGene_Generates_At_Least_One_University_In_1000_Tries()
        {
            var gene = Building.GetRandom(Epoch.Third);
            Assert.IsInstanceOf<University>(gene);
        }

        [Test]
        [Retry(1000)]
        public void At_Third_Epoch_GetRandomGene_Generates_At_Least_One_Port_In_1000_Tries()
        {
            var gene = Building.GetRandom(Epoch.Third);
            Assert.IsInstanceOf<Port>(gene);
        }

        [Test]
        public void When_Building_Is_Under_The_Horizontal_Road_Its_Direction_Is_Set_To_Down()
        {
            var road = new Road(new[]
            {
                new Point(0,1),
                new Point(1,1)
            });
            var building = new Residence()
            {
                Position = new Point(0, 0)
            };

            road.AddBuilding(building);

            Assert.AreEqual(Direction.Down, building.Direction);
        }

        [Test]
        public void When_Building_Is_Above_The_Horizontal_Road_Its_Direction_Is_Set_To_Up()
        {
            var road = new Road(new[]
            {
                new Point(0,1),
                new Point(1,1)
            });
            var building = new Residence()
            {
                Position = new Point(0, 2)
            };

            road.AddBuilding(building);

            Assert.AreEqual(Direction.Up, building.Direction);
        }

        [Test]
        public void When_Building_Is_On_The_Left_Side_The_Vertical_Road_Its_Direction_Is_Set_To_Left()
        {
            var road = new Road(new[]
            {
                new Point(1,0),
                new Point(1,1)
            });
            var building = new Residence()
            {
                Position = new Point(0, 0)
            };

            road.AddBuilding(building);

            Assert.AreEqual(Direction.Left, building.Direction);
        }

        [Test]
        public void When_Building_Is_On_The_Right_Side_The_Vertical_Road_Its_Direction_Is_Set_To_Right()
        {
            var road = new Road(new[]
            {
                new Point(1,0),
                new Point(1,1)
            });
            var building = new Residence()
            {
                Position = new Point(2, 0)
            };

            road.AddBuilding(building);

            Assert.AreEqual(Direction.Right, building.Direction);
        }
    }
}
