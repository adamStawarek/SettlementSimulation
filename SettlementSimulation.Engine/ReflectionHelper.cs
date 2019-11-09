using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SettlementSimulation.Engine
{
    public static class ReflectionHelper
    {
        public static IEnumerable<T> GetAllObjectsByType<T>()
        {
            var result = Assembly.GetAssembly(typeof(SimulationEngine)).GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T)))
                .Select(t => (T) Activator.CreateInstance(t));
            return result;
        }
    }
}