using System;
using rnd = SettlementSimulation.Engine.RandomProvider;

namespace SettlementSimulation.Engine
{
    public class Dna<T>
    {
        public T[] Genes { get; }
        public float Fitness { get; set; }
        private readonly Func<T> _getRandomGene;
        private readonly Func<int, float> _fitnessFunction;

        public Dna(int size, Func<T> getRandomGene, Func<int, float> fitnessFunction, bool shouldInitGenes = true)
        {
            _getRandomGene = getRandomGene;
            _fitnessFunction = fitnessFunction;
            Genes = new T[size];
            if (!shouldInitGenes) return;
            for (int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = _getRandomGene();
            }
        }

        public float CalculateFitness(int index)
        {
            return Fitness = _fitnessFunction(index);
        }

        public Dna<T> Crossover(Dna<T> otherParent)
        {
            var child = new Dna<T>(Genes.Length, _getRandomGene, _fitnessFunction, false);
            for (int i = 0; i < Genes.Length; i++)
            {
                child.Genes[i] = rnd.NextDouble() < 0.5 ? Genes[i] : otherParent.Genes[i];
            }

            return child;
        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (rnd.NextDouble() < mutationRate)
                {
                    Genes[i] = _getRandomGene();
                }
            }
        }
    }
}