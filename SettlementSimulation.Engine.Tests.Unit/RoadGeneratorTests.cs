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
            _fields = new Field[4, 5];

            _fields[0, 0] = new Field();
            _fields[1, 0] = new Field();
            _fields[2, 0] = new Field();
            _fields[3, 0] = new Field() { InSettlement = true };

            _fields[0, 1] = new Field();
            _fields[1, 1] = new Field();
            _fields[2, 1] = new Field();
            _fields[3, 1] = new Field() { InSettlement = true };

            _fields[0, 2] = new Field();
            _fields[1, 2] = new Field();
            _fields[2, 2] = new Field();
            _fields[3, 2] = new Field() { InSettlement = true };

            _fields[0, 3] = new Field() { InSettlement = true };
            _fields[1, 3] = new Field() { InSettlement = true };
            _fields[2, 3] = new Field() { InSettlement = true };
            _fields[3, 3] = new Field() { InSettlement = true };

            _fields[0, 4] = new Field() { InSettlement = true };
            _fields[1, 4] = new Field() { InSettlement = true };
            _fields[2, 4] = new Field() { InSettlement = true };
            _fields[3, 4] = new Field() { InSettlement = true };

            _roadGenerator = new RoadGenerator();
        }

        [Test]
        public void GenerateRoadPoints_Returns_Shortest_Path()
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

            var path = _roadGenerator.Generate(new RoadGenerationTwoPoints()
            {
                Fields = _fields,
                BlockedCells = new []{ new Point(1, 4) },
                Start = new Point(0, 4),
                End = new Point(3, 0)
            });

            CollectionAssert.AreEquivalent(expectedPath, path);
        }

        [Test]
        public void GenerateAttached_When_New_Road_Does_Not_Cross_Other_Ones()
        {
            var road = new Road(new[]
            {
                new Point(3, 0),
                new Point(3, 1),
                new Point(3, 2),
                new Point(3, 3)
            });

            road.AddBuilding(new Residence() {Position = new Point(2, 3)});
            var roadPoints = _roadGenerator.GenerateAttached(new RoadGenerationAttached()
            {
                Fields = _fields,
                Road = road,
                Roads = new List<IRoad> {road}
            });

            
        }
    }
}