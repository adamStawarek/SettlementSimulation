using System.Collections.Generic;
using System.Linq;
using rnd = GeneticAlgorithm.RandomProvider;

namespace GeneticAlgorithm
{
    public abstract class BaseGeneticEngine<T>
    {
        public List<Dna<T>> Population { get; set; }
        public int Generation { get; set; }
        public float MutationRate { get; set; }
        public float BestFitness { get; private set; }
        public T[] BestGenes { get; }

        private float _fitnessSum;

        protected BaseGeneticEngine(int populationSize, int dnaSize, float mutationRate = 0.01f)
        {
            Generation = 1;
            MutationRate = mutationRate;
            Population = new List<Dna<T>>(populationSize);
            BestGenes = new T[dnaSize];
            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new Dna<T>(dnaSize, GetRandomGene, SubjectFitness));
            }
        }

        public void NewGeneration()
        {
            if (!Population.Any()) return;

            CalculateFitness();

            var newPopulation = new List<Dna<T>>();

            for (int i = 0; i < Population.Count; i++)
            {
                Dna<T> parent1 = ChooseParent();
                Dna<T> parent2 = ChooseParent();

                Dna<T> child = parent1.Crossover(parent2);

                child.Mutate(MutationRate);

                newPopulation.Add(child);
            }

            Population = newPopulation;

            Generation++;
        }

        public abstract T GetRandomGene();

        public abstract float SubjectFitness(int index);

        private void CalculateFitness()
        {
            _fitnessSum = 0;
            Dna<T> best = Population[0];
            Population.ForEach(p =>
            {
                _fitnessSum += p.CalculateFitness(Population.IndexOf(p));
                best = best.Fitness < p.Fitness ? p : best;
            });

            BestFitness = best.Fitness;
            best.Genes.CopyTo(BestGenes, 0);
        }

        private Dna<T> ChooseParent()
        {
            double fitness = rnd.NextDouble() * _fitnessSum;
            foreach (var subject in Population)
            {
                if (fitness < subject.Fitness)
                {
                    return subject;
                }

                fitness -= subject.Fitness;
            }

            return Population.First();
        }
    }
}