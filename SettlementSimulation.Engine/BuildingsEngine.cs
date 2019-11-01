using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GeneticAlgorithm
{
    public class BuildingsEngine : BaseGeneticEngine<Building>
    {
        public Epoch currentEpoch { get; set; }

        public BuildingsEngine(int populationSize, int dnaSize, float mutationRate = 0.01F) : base(populationSize, dnaSize, mutationRate)
        {
            currentEpoch = Epoch.First;
        }

        public override Building GetRandomGene()
        {
            var buildings = new List<Building>();
            switch (currentEpoch)
            {
                case Epoch.First:
                    {
                        buildings.AddRange(GetAllObjectsByType<FirstTypeBuilding>());
                        break;
                    }
                case Epoch.Second:
                    {
                        buildings.AddRange(GetAllObjectsByType<FirstTypeBuilding>());
                        buildings.AddRange(GetAllObjectsByType<SecondTypeBuilding>());
                        break;
                    }
                case Epoch.Third:
                    {
                        buildings.AddRange(GetAllObjectsByType<FirstTypeBuilding>());
                        buildings.AddRange(GetAllObjectsByType<SecondTypeBuilding>());
                        buildings.AddRange(GetAllObjectsByType<ThirdTypeBuilding>());
                        break;
                    }
            }

            var diceRoll = RandomProvider.NextDouble();
            double cumulative = 0.0;
            for (int i = 0; i < buildings.Count; i++)
            {
                cumulative += buildings[i].Probability;
                if (diceRoll < cumulative)
                {
                    return buildings[i];
                }
            }
            return new EmptyArea();
        }

        public override float SubjectFitness(int index)
        {
            float score = 0;
            Dna<Building> dna = this.Population[index];
            return score;
        }

        private IEnumerable<T> GetAllObjectsByType<T>()
        {
            var result = Assembly.GetAssembly(typeof(BuildingsEngine)).GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T)))
                .Select(t => (T)Activator.CreateInstance(t));
            return result;
        }

    }
}
