using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.AreaGenerator.Models.Terrains;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Tests.Unit
{
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public class DnaTests
    {
        private Field[,] _fields;
        private IEnumerable<Point> _mainRoad;

        [SetUp]
        public void SetUp()
        {
            _fields = new Field[50, 50];
            int waterLevel = 10;

            for (int i = 0; i < _fields.GetLength(0); i++)
            {
                for (int j = 0; j < _fields.GetLength(1); j++)
                {
                    var position = new Point(i, j);
                    if (j < waterLevel)
                    {
                        _fields[i, j] = new Field()
                        {
                            InSettlement = false,
                            Position = position,
                            Terrain = new Water()
                        };
                    }
                    else
                    {
                        _fields[i, j] = new Field()
                        {
                            InSettlement = true,
                            Position = position,
                            Terrain = new Lowland(),
                            DistanceToWater = position.Y - waterLevel,
                            DistanceToMainRoad = position.Y - waterLevel - 1
                        };
                    }
                }
            }

            var roadGenerator = new RoadGenerator();
            _mainRoad = roadGenerator.GenerateStraight(new Models.RoadGenerationTwoPoints()
            {
                Start = new Point(0, waterLevel),
                End = new Point(_fields.GetLength(0) - 1, waterLevel),
                Fields = _fields
            });
        }

        [Test]
        public void InitializeGenes_Does_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(() => new Dna(_fields, _mainRoad, shouldInitGenes: true));
        }
    }
}