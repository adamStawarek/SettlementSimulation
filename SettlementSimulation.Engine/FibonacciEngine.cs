namespace GeneticAlgorithm
{
    public class FibonacciEngine : BaseGeneticEngine<int>
    {
        public FibonacciEngine(int populationSize, int dnaSize, float mutationRate) : base(populationSize, dnaSize, mutationRate)
        {
        }

        public override int GetRandomGene()
        {
            return RandomProvider.Next(50);
        }

        public override float SubjectFitness(int index)
        {
            float score = 0;
            Dna<int> dna = this.Population[index];
            for (int i = 0; i < dna.Genes.Length; i++)
            {
                if (i == 0 && dna.Genes[i] == 0 ||
                    i == 1 && dna.Genes[i] == 1)
                {
                    score += 1;
                }
                else if (i > 1)
                {
                    bool isFibonacci = true;
                    for (int j = 2; j < i; j++)
                    {
                        if (dna.Genes[j - 2] + dna.Genes[j - 1] != dna.Genes[j])
                        {
                            isFibonacci = false;
                        }
                    }

                    if (isFibonacci)
                    {
                        score += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            score /= dna.Genes.Length;
            return score;
        }
    }
}