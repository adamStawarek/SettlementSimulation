using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Enumerators;

namespace SettlementSimulation.Engine.Models.Buildings
{
    public abstract class Building : IBuilding, ICopyable<Building>
    {
        public abstract double Probability { get; }
        public abstract double CalculateFitness(BuildingRule model);
        public abstract int Space { get; }

        public Direction? Direction { get; set; }
        public int Age { get; set; }
        public Point Position { get; set; }
        public double? Fitness { get; set; }

        public Building Copy()
        {
            var copy = (Building)Activator.CreateInstance(this.GetType());
            copy.Position = this.Position;
            return copy;
        }

        public virtual IRoad Road { get; set; }

        public void SetFitness(BuildingRule model)
        {
            Fitness = this.CalculateFitness(model);
        }
        public static Building GetRandom(Epoch epoch)
        {
            var buildings = Assembly.GetAssembly(typeof(SimulationEngine))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Building)) &&
                            t.GetCustomAttributes(typeof(EpochAttribute), false)
                                .Cast<EpochAttribute>()
                                .Any(a => a.Epoch <= epoch))
                .Select(t => (Building) Activator.CreateInstance(t))
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
        public override string ToString()
        {
            return $"{nameof(Type)}: {this.GetType().Name} " +
                   $"{nameof(Position)}: {Position}";
        }

    }
}