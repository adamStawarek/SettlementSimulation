using NUnit.Framework;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class RoadTests
    {
        [Test]
        public void GetPossiblePositionsToAttachBuilding_When_There_Are_No_Buildings_And_Any_Already_Attached_Roads()
        {
            var road = new Road(new[]
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            });

            var expected = new[]
            {
                new Point(0, 0),
                new Point(0, 1),
                new Point(0, 2),
                new Point(0, 3),

                new Point(2, 0),
                new Point(2, 1),
                new Point(2, 2),
                new Point(2, 3)
            };

            var actual = road.GetPossiblePositionsToAttachBuilding(new System.Collections.Generic.List<Interfaces.IRoad>() { road });
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachRoad_When_There_Are_No_Buildings_And_Any_Already_Attached_Roads()
        {
            var road = new Road(new[]
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            });

            var expected = new[]
            {
                new Point(0, 0),
                new Point(0, 1),
                new Point(0, 2),
                new Point(0, 3),

                new Point(2, 0),
                new Point(2, 1),
                new Point(2, 2),
                new Point(2, 3)
            };

            var actual = road.GetPossiblePositionsToAttachRoad(new System.Collections.Generic.List<Interfaces.IRoad>() { road });
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachBuilding_When_There_Are_Buildings_But_No_Already_Attached_Roads()
        {
            var road = new Road(new[]
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            });

            road.AddBuilding(new Residence() { Position = new Point(0, 0) });
            road.AddBuilding(new Residence() { Position = new Point(0, 2) });
            road.AddBuilding(new Residence() { Position = new Point(2, 2) });

            var expected = new[]
            {
                new Point(0, 1),
                new Point(0, 3),

                new Point(2, 0),
                new Point(2, 1),
                new Point(2, 3)
            };

            var actual = road.GetPossiblePositionsToAttachBuilding(new System.Collections.Generic.List<Interfaces.IRoad>() { road });
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachRoad_When_There_Are_Buildings_But_No_Already_Attached_Roads()
        {
            var road = new Road(new[]
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            });

            road.AddBuilding(new Residence() { Position = new Point(0, 0) });
            road.AddBuilding(new Residence() { Position = new Point(0, 2) });
            road.AddBuilding(new Residence() { Position = new Point(2, 2) });

            var expected = new[]
            {
                new Point(0, 1),
                new Point(0, 3),

                new Point(2, 0),
                new Point(2, 1),
                new Point(2, 3)
            };

            var actual = road.GetPossiblePositionsToAttachRoad(new System.Collections.Generic.List<Interfaces.IRoad>() { road });
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachBuilding_When_There_Are_No_Buildings_There_Are_Already_Attached_Road_On_One_Side_Of_The_Road()
        {
            var road = new Road(new[]
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            });

            var road2 = new Road(new[]
            {
                new Point(2, 2),
                new Point(3, 2)
            });

            var expected = new[]
            {
                new Point(0, 0),
                new Point(0, 1),
                new Point(0, 2),
                new Point(0, 3),

                new Point(2, 0),
                new Point(2, 1),
                new Point(2, 3)

            };

            var actual = road.GetPossiblePositionsToAttachBuilding(
                new System.Collections.Generic.List<Interfaces.IRoad>(){road, road2});
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachRoad_When_There_Are_No_Buildings_There_Are_Already_Attached_Road_On_One_Side_Of_The_Road()
        {
            var road = new Road(new[]
            {
                new Point(2, 0),
                new Point(2, 1),
                new Point(2, 2),
                new Point(2, 3)
            });

            var road2 = new Road(new[]
            {
                new Point(0, 3),
                new Point(1, 3)
            });
           

            var expected = new[]
            {
                new Point(1, 0),
                new Point(3, 0),
                new Point(3, 1),
                new Point(3, 2),
                new Point(3, 3)
            };

            var actual = road.GetPossiblePositionsToAttachRoad(new System.Collections.Generic.List<Interfaces.IRoad>() { road, road2 }, 3);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachRoad_When_There_Are_No_Buildings_There_Are_Already_Attached_Road_On_Both_Sides_Of_The_Road()
        {
            var road = new Road(new[]
            {
                new Point(2, 0),
                new Point(2, 1),
                new Point(2, 2),
                new Point(2, 3)
            });

            var road2 = new Road(new[]
            {
                new Point(0, 3),
                new Point(1, 3)
            });
            var road3 = new Road(new[]
            {
                new Point(3, 3),
                new Point(4,3)
            });

            var expected = new[]
            {
                new Point(1, 0),
                new Point(3, 0)
            };

            var actual = road.GetPossiblePositionsToAttachRoad(new System.Collections.Generic.List<Interfaces.IRoad>() { road, road2, road3 }, 3);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachRoad_When_There_Are_Buildings_There_Are_Already_Attached_Road_On_Both_Sides_Of_The_Road()
        {
            var road = new Road(new[]
            {
                new Point(2, 0),
                new Point(2, 1),
                new Point(2, 2),
                new Point(2, 3)
            });

            road.AddBuilding(new Residence() { Position = new Point(1, 0) });
            var road2 = new Road(new[]
            {
                new Point(0, 3),
                new Point(1, 3)
            });
            var road3 = new Road(new[]
            {
                new Point(3, 3),
                new Point(4,3)
            });

            var expected = new[]
            {
                new Point(3, 0)
            };

            var actual = road.GetPossiblePositionsToAttachRoad(new System.Collections.Generic.List<Interfaces.IRoad>() { road, road2, road3 },3);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void Copy_Returns_Road_With_TheSame_Number_Of_Buildings()
        {
            var road = new Road(new[]
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            });

            road.AddBuilding(new Residence() { Position = new Point(0, 0) });
            road.AddBuilding(new Residence() { Position = new Point(0, 2) });
            road.AddBuilding(new Residence() { Position = new Point(2, 2) });

            var copy = road.Copy();

            Assert.AreEqual(3, copy.Buildings.Count);
        }
    }
}