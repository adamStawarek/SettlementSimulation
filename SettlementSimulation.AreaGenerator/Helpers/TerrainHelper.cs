﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SettlementSimulation.AreaGenerator.Interfaces;

namespace SettlementSimulation.AreaGenerator.Helpers
{
    public class TerrainHelper
    {
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
            var orderedTerrains = this.GetAllTerrains().OrderBy(t => t.UpperBound).ToList();
            return orderedTerrains.First(f => f.UpperBound >= height);
        }

        public IEnumerable<ITerrain> GetAllTerrains()
        {
            return Assembly.Load("SettlementSimulation.AreaGenerator")
                .GetTypes()
                .Where(t => t.GetInterface(nameof(ITerrain))!=null && !t.IsInterface)
                .Select(t=>(ITerrain)Activator.CreateInstance(t)).ToList();
        }
    }
}