using NUnit.Framework;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class RoadTests
    {
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

            var actual = road.GetPossiblePositionsToAttachRoad();
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

            var actual = road.GetPossiblePositionsToAttachRoad();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachRoad_When_There_Are_No_Buildings_There_Are_Already_Attached_Road_On_One_Side_Of_The_Road()
        {
            var road = new Road(new[]
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            });

            road.BlockedCells.Add(new Point(2, 2));

            var expected = new[]
            {
                new Point(0, 0),
                new Point(0, 1),
                //new Point(0, 2),TODO maybe???
                new Point(0, 3),

            };

            var actual = road.GetPossiblePositionsToAttachRoad();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachRoad_When_There_Are_No_Buildings_There_Are_Already_Attached_Road_On_Both_Sides_Of_The_Road()
        {
            var road = new Road(new[]
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            });

            road.BlockedCells.Add(new Point(0, 3));
            road.BlockedCells.Add(new Point(2, 3));


            var expected = new[]
            {
                new Point(0, 0),
                new Point(2, 0)
            };

            var actual = road.GetPossiblePositionsToAttachRoad(3);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetPossiblePositionsToAttachRoad_When_There_Are_Buildings_There_Are_Already_Attached_Road_On_Both_Sides_Of_The_Road()
        {
            var road = new Road(new[]
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(1, 3)
            });

            road.AddBuilding(new Residence() {Position = new Point(0, 0)});
            road.BlockedCells.Add(new Point(0, 3));
            road.BlockedCells.Add(new Point(2, 3));


            var expected = new[]
            {
                new Point(2, 0)
            };

            var actual = road.GetPossiblePositionsToAttachRoad(3);
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}