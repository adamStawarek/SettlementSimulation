using NUnit.Framework;
using SettlementSimulation.AreaGenerator.Helpers;
using SettlementSimulation.AreaGenerator.Interfaces;
using SettlementSimulation.AreaGenerator.Models.Terrains;

namespace SettlementSimulation.AreaGenerator.Tests.Unit
{
    public class TerrainHelperTests
    {
        [TestCase(146)]
        [TestCase(150)]
        [TestCase(170)]
        public void GetTerrainForHeight_Returns_Lowland_When_Height_Is_From_145_To_170(byte height)
        {
            var terrainHelper = new TerrainHelper();
            ITerrain terrain = terrainHelper.GetTerrainForHeight(height);
            Assert.IsInstanceOf<Lowland>(terrain);
        }
    }
}