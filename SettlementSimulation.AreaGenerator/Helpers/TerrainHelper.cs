using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SettlementSimulation.AreaGenerator.Interfaces;
using SettlementSimulation.AreaGenerator.Models.Terrains;

namespace SettlementSimulation.AreaGenerator.Helpers
{
    public class TerrainHelper
    {
        private readonly List<ITerrain> _terrains;

        public TerrainHelper()
        {
            var allTerrains = new List<ITerrain>
            {
                new DeepWater(),
                new Water(),
                new Sand(),
                new Lowland(),
                new HighGround(),
                new MountainBottom(),
                new MountainTop()
            };

            _terrains = allTerrains.OrderBy(t => t.UpperBound).ToList();
        }

        public ITerrain GetTerrain<T>() where T : ITerrain
        {
            return (ITerrain)Assembly.Load("SettlementSimulation.AreaGenerator")
                .GetTypes()
                .Where(t => t == typeof(T))
                .Select(Activator.CreateInstance)
                .FirstOrDefault();
        }

        public ITerrain GetTerrainForHeight(byte height)
        {
            return _terrains.First(f => f.UpperBound >= height);
        }

        public IEnumerable<ITerrain> GetAllTerrains()
        {
            return _terrains;
        }
    }
}