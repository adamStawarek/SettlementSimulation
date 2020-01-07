using System.Collections.Generic;
using NUnit.Framework;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings.FirstType;

namespace SettlementSimulation.Engine.Tests.Unit
{
    public class RoadGeneratorTests
    {
        private Field[,] _fields;
        private IRoadGenerator _roadGenerator;

        [SetUp]
        public void SetUp()
        {
            _fields = new Field[5, 5];

            _fields[0, 0] = new Field();
            _fields[1, 0] = new Field();
            _fields[2, 0] = new Field();
            _fields[3, 0] = new Field() { InSettlement = true };
            _fields[4, 0] = new Field() { InSettlement = true };

            _fields[0, 1] = new Field();
            _fields[1, 1] = new Field();
            _fields[2, 1] = new Field();
            _fields[3, 1] = new Field() { InSettlement = true };
            _fields[4, 1] = new Field() { InSettlement = true };

            _fields[0, 2] = new Field();
            _fields[1, 2] = new Field();
            _fields[2, 2] = new Field();
            _fields[3, 2] = new Field() { InSettlement = true };
            _fields[4, 2] = new Field() { InSettlement = true };

            _fields[0, 3] = new Field() { InSettlement = true };
            _fields[1, 3] = new Field() { InSettlement = true };
            _fields[2, 3] = new Field() { InSettlement = true };
            _fields[3, 3] = new Field() { InSettlement = true };
            _fields[4, 3] = new Field() { InSettlement = true };

            _fields[0, 4] = new Field() { InSettlement = true };
            _fields[1, 4] = new Field() { InSettlement = true };
            _fields[2, 4] = new Field() { InSettlement = true };
            _fields[3, 4] = new Field() { InSettlement = true };
            _fields[4, 4] = new Field() { InSettlement = true };

            _roadGenerator = new RoadPointsGenerator();
        }

        [Test]
        [Repeat(10)]
        public void Generate_Returns_Shortest_Path()
        {
            var expectedPath = new List<Point>()
            {
                new Point(0, 4),
                new Point(0, 3),
                new Point(1, 3),
                new Point(2, 3),
                new Point(3, 3),
                new Point(3, 2),
                new Point(3, 1),
                new Point(3, 0)
            };

            _fields[1, 4].IsBlocked = true;

            var path = _roadGenerator.Generate(new RoadGenerationTwoPoints()
            {
                Fields = _fields,
                Start = new Point(0, 4),
                End = new Point(3, 0)
            });

            CollectionAssert.AreEquivalent(expectedPath, path);
        }

        [Test]
        [Repeat(10)]
        public void GenerateAttached_With_Existing_Single_Road()
        {
            var road = new Road(new[]
            {
                new Point(0, 3),
                new Point(0, 4)
            });

            road.AddBuilding(new Residence() { Position = new Point(1, 3) });

            var actual = _roadGenerator.GenerateAttached(new RoadGenerationAttached()
            {
                Fields = _fields,
                Road = road,
                Roads = new List<IRoad>() { road },
                MinRoadLength = 4,
                MaxRoadLength = 5
            });

            var expected = new[]
            {
                new Point(1, 4),
                new Point(2, 4),
                new Point(3, 4),
                new Point(4, 4)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        [Repeat(10)]
        public void GenerateAttached_With_Existing_Multiple_Roads_And_Single_Intersection_Point()
        {
            var road = new Road(new[]
            {
                new Point(0, 3),
                new Point(0, 4)
            });
            road.AddBuilding(new Residence() { Position = new Point(1, 3) });

            var road2 = new Road(new[]
            {
                new Point(3, 0),
                new Point(3, 1),
                new Point(3, 2),
                new Point(3, 3),
                new Point(3,4)
            });


            var actual = _roadGenerator.GenerateAttached(new RoadGenerationAttached()
            {
                Fields = _fields,
                Road = road,
                Roads = new List<IRoad>() { road, road2 },
                MinRoadLength = 2,
                MinDistanceBetweenRoads = 3,
                MaxRoadLength = 50
            });

            var expected = new[]
            {
                new Point(1, 4),
                new Point(2, 4)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        [Repeat(10)]
        public void GenerateAttached_Horizontal_With_Existing_Multiple_Roads_And_Two_Intersection_Points()
        {
            var road = new Road(new[]
            {
                new Point(0, 3),
                new Point(0, 4)
            });
            road.AddBuilding(new Residence() { Position = new Point(1, 3) });


            var road2 = new Road(new[]
            {
                new Point(2, 3),
                new Point(2, 4)
            });

            var road3 = new Road(new[]
            {
                new Point(4, 3),
                new Point(4, 4)
            });

            var actual = _roadGenerator.GenerateAttached(new RoadGenerationAttached()
            {
                Fields = _fields,
                Road = road,
                Roads = new List<IRoad>() { road, road2 ,road3},
                MinRoadLength = 1,
                MinDistanceBetweenRoads = 3,
                MaxRoadLength = 100
            });

            var expected = new[]
            {
                new Point(1, 4)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        [Repeat(10)]
        public void GenerateAttached_Vertical_With_Existing_Multiple_Roads_And_Two_Intersection_Points()
        {
            var road = new Road(new[]
            {
                new Point(3, 0),
                new Point(4, 0)
            });
            road.AddBuilding(new Residence() { Position = new Point(3, 1) });

            var road2 = new Road(new[]
            {
                new Point(3, 2),
                new Point(4, 2)
            });

            var road3 = new Road(new[]
            {
                new Point(3, 4),
                new Point(4, 4)
            });

            var actual = _roadGenerator.GenerateAttached(new RoadGenerationAttached()
            {
                Fields = _fields,
                Road = road,
                Roads = new List<IRoad>() { road, road2, road3 },
                MinRoadLength = 1,
                MinDistanceBetweenRoads = 3,
                MaxRoadLength = 100
            });

            var expected = new[]
            {
                new Point(4, 1)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}