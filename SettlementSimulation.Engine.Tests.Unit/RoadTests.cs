using NUnit.Framework;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using System.Collections.Generic;
using SettlementSimulation.Engine.Interfaces;

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

            var actual = road.GetPossiblePositionsToAttachRoad(new List<Interfaces.IRoad>() { road });
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

            var actual = road.GetPossiblePositionsToAttachRoad(
                new List<Interfaces.IRoad>() { road, road2 },
                2);
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

            var actual = road.GetPossiblePositionsToAttachRoad(
                new List<Interfaces.IRoad>() { road, road2, road3 }, 
                2);
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

            var actual = road.GetPossiblePositionsToAttachRoad(
                new List<Interfaces.IRoad>() { road, road2, road3 },
                2);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachRoad__Single_Adjacent_Road_Test()
        {
            var road1 = new Road(new[]
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(2, 1),
                new Point(3, 1),
                new Point(4, 1)
            });

            var road2 = new Road(new[]
            {
                new Point(1, 2),
                new Point(1, 3)
            });

            var road3 = new Road(new[]
            {
                new Point(0, 4),
                new Point(1, 4),
                new Point(2, 4),
                new Point(3, 4),
                new Point(4, 4)
            });

            var actual = road3.GetPossiblePositionsToAttachRoad(
                new List<Interfaces.IRoad> {road1, road2, road3},
                minDistanceBetweenRoads: 1);
                
            var expected = new[]
            {
                new Point(4,3),
                new Point(3,3),
                new Point(0, 5),
                new Point(1, 5),
                new Point(2, 5),
                new Point(3, 5),
                new Point(4, 5)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachRoad__Multiple_Adjacent_Road_Test()
        {
            var road1 = new Road(new[]
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(2, 1),
                new Point(3, 1),
                new Point(4, 1)
            });

            var road2 = new Road(new[]
            {
                new Point(1, 2),
                new Point(1, 3)
            });

            var road3 = new Road(new[]
            {
                new Point(3, 2),
                new Point(3, 3)
            });

            var road4 = new Road(new[]
            {
                new Point(4, 2),
                new Point(4, 3)
            });

            var road5 = new Road(new[]
            {
                new Point(3, 5),
                new Point(3, 6)
            });

            var road6 = new Road(new[]
            {
                new Point(0, 4),
                new Point(1, 4),
                new Point(2, 4),
                new Point(3, 4),
                new Point(4, 4)
            });

            var actual = road6.GetPossiblePositionsToAttachRoad(
                new List<Interfaces.IRoad> { road1, road2, road3, road4, road5, road6 },
                minDistanceBetweenRoads: 1);

            var expected = new[]
            {
                new Point(0,5),
                new Point(1,5)
               
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void AttachedRoads_Returns_All_Roads_That_Are_Directly_Attached()
        {
            var road1 = new Road(new[]
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(2, 1),
                new Point(3, 1),
                new Point(4, 1)
            });

            var road2 = new Road(new[]
            {
                new Point(1, 2),
                new Point(1, 3)
            });

            var road3 = new Road(new[]
            {
                new Point(3, 2),
                new Point(3, 3)
            });

            var road4 = new Road(new[]
            {
                new Point(4, 2),
                new Point(4, 3)
            });

            var road5 = new Road(new[]
            {
                new Point(3, 5),
                new Point(3, 6)
            });

            var road6 = new Road(new[]
            {
                new Point(0, 4),
                new Point(1, 4),
                new Point(2, 4),
                new Point(3, 4),
                new Point(4, 4)
            });

            var allRoads = new List<IRoad> {road1, road2, road3, road4, road5, road6};
            Assert.Multiple(() =>
            {
                CollectionAssert.AreEquivalent(
                    new[] { road2, road3, road4 },
                    road1.AttachedRoads(allRoads));

                CollectionAssert.AreEquivalent(
                    new[] { road1, road6 },
                    road2.AttachedRoads(allRoads));

                CollectionAssert.AreEquivalent(
                    new IRoad[] { },
                    road5.AttachedRoads(allRoads));
            });
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