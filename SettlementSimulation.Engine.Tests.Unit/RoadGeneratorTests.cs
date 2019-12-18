using System.Collections.Generic;
using NUnit.Framework;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;
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
        public void Generate_Returns_Shortest_Path()
        {
            var structures = new List<IStructure>()
            {
                new Residence() {Location = new Location(1, 4)}
            };

            var expectedPath = new List<Point>()
            {
                new Point(0, 4),
                new Point(0, 3),
                new Point(1, 3),
                new Point(2, 3),
                new Point(3, 3),
                new Point(3, 2),
                new Point(3, 0)
            };

            var path = _roadGenerator.Generate(new RoadGenerationInfo()
            {
                Fields = _fields,
                Structures = structures,
                Start = new Point(0, 4),
                End = new Point(3, 0)
            });

            CollectionAssert.AreEquivalent(expectedPath, path);
        }

    }
}