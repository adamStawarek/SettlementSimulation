using System;
using System.Linq;
using System.Reflection;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Models.Buildings
{
    public abstract class Building : IStructure
    {
        public Location Location { get; set; }
        public abstract double Probability { get; }

        public static Building GetRandom(Epoch epoch)
        {
            var buildings = Assembly.GetAssembly(typeof(SimulationEngine)).GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Building)) &&
                            t.GetCustomAttributes(typeof(EpochAttribute), false)
                                .Cast<EpochAttribute>()
                                .Any(a => a.Epoch == epoch))
                .Select(t=>(Building)Activator.CreateInstance(t))
                .ToList();

            var diceRoll = RandomProvider.NextDouble();
            var cumulative = 0.0;
            foreach (var building in buildings)
            {
                cumulative += building.Probability;
                if (diceRoll < cumulative)
                {
                    return building;
                }
            }

            return buildings.OrderByDescending(b => b.Probability).First();
        }
    }
}