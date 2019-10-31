using System;
using System.Linq;
using System.Threading;
using rnd = GeneticAlgorithm.RandomProvider;

namespace GeneticAlgorithm
{
    class Program
    {
        private static readonly int[] Fibonacci = { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34 };

        static void Main()
        {
            var fibonacciEngine = new FibonacciEngine(500, Fibonacci.Length, 0.01f);
            while (true)
            {
                fibonacciEngine.NewGeneration();
                Console.WriteLine(string.Join("-",fibonacciEngine.BestGenes));
                Thread.Sleep(10);
                if (fibonacciEngine.BestGenes.SequenceEqual(Fibonacci))
                {
                    break;
                }
            }
        }
    }
}
