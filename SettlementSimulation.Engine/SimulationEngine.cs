using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine
{
    public class SimulationEngine
    {
        #region private fields
        private float _fitnessSum;
        private Epoch _currentEpoch;
        private readonly Stack<Epoch> _allEpochs;
        #endregion

        #region properties
        public List<Dna> Population { get; set; }
        public int Generation { get; set; }
        public float BestFitness { get; set; }
        public IStructure[] BestGenes { get; }
        public Field[,] Fields { get; set; }
        public List<Point> MainRoad { get; set; }
        #endregion

        public SimulationEngine(
            int populationSize,
            int dnaSize,
            Field[,] fields,
            List<Point> mainRoad)
        {
            Fields = fields;
            MainRoad = mainRoad;
            Generation = 1;
            Population = new List<Dna>(populationSize);
            BestGenes = new IStructure[dnaSize];
            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new Dna(dnaSize, fields, mainRoad));
            }

            _allEpochs = new Stack<Epoch>();
            _allEpochs.Push(Epoch.Third);
            _allEpochs.Push(Epoch.Second);
            _allEpochs.Push(Epoch.First);
            SetNextEpoch();
        }

        public Epoch SetNextEpoch()
        {
            _currentEpoch = _allEpochs.Pop();
            return _currentEpoch;
        }

        public void NewGeneration()
        {
            if (!Population.Any()) return;

            CalculateFitness();

            var newPopulation = new List<Dna>();

            for (int i = 0; i < Population.Count; i++)
            {
                Dna parent1 = ChooseParent();
                Dna parent2 = ChooseParent();

                Dna child = parent1.Crossover(parent2);

                child.Mutate(_currentEpoch);

                newPopulation.Add(child);
            }

            Population = newPopulation;

            Generation++;
        }

        public void CalculateFitness()
        {
            _fitnessSum = 0;
            var best = Population[0];
            Population.ForEach(p =>
            {
                _fitnessSum += p.CalculateFitness(_currentEpoch);
                best = best.Fitness < p.Fitness ? p : best;
            });

            BestFitness = best.Fitness;
            best.Genes.CopyTo(BestGenes, 0);
        }

        public Dna ChooseParent()
        {
            double fitness = RandomProvider.NextDouble() * _fitnessSum;
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