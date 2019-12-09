using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using SettlementSimulation.Engine.Rules;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static SettlementSimulation.Engine.Helpers.ReflectionHelper;
using rnd = SettlementSimulation.Engine.Helpers.RandomProvider;

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
        public List<Dna<IStructure>> Population { get; set; }
        public int Generation { get; set; }
        public float MutationRate { get; }
        public float BestFitness { get; set; }
        public IStructure[] BestGenes { get; }
        public List<IRule> Rules { get; set; }
        public Field[,] Fields { get; set; }
        public List<Point> MainRoad { get; set; }
        #endregion

        public SimulationEngine(int populationSize, int dnaSize, Field[,] fields, List<Point> mainRoad)
        {
            Fields = fields;
            MainRoad = mainRoad;
            Generation = 1;
            MutationRate = 0.01F;
            Population = new List<Dna<IStructure>>(populationSize);
            BestGenes = new IStructure[dnaSize];
            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new Dna<IStructure>(dnaSize, GetRandomGene, SubjectFitness));
            }

            _allEpochs = new Stack<Epoch>();
            _allEpochs.Push(Epoch.Third);
            _allEpochs.Push(Epoch.Second);
            _allEpochs.Push(Epoch.First);
            SetNextEpoch();

            Rules = GetAllObjectsByType<IRule>().ToList();
        }

        public void AddRules(IEnumerable<IRule> rules)
        {
            Rules.RemoveAll(r => rules.Any(r2 => r2.GetType() == r.GetType()));
            Rules.AddRange(rules);
        }

        public Epoch SetNextEpoch()
        {
            _currentEpoch = _allEpochs.Pop();
            return _currentEpoch;
        }

        public IStructure GetRandomGene()
        {
            var buildings = new List<Building>();
            switch (_currentEpoch)
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

            var diceRoll = rnd.NextDouble();
            double cumulative = 0.0;
            foreach (var building in buildings)
            {
                cumulative += building.Probability;
                if (diceRoll < cumulative)
                {
                    return building;
                }
            }

            return new Residence();
        }

        public void NewGeneration()
        {
            if (!Population.Any()) return;

            CalculateFitness();

            var newPopulation = new List<Dna<IStructure>>();

            for (int i = 0; i < Population.Count; i++)
            {
                Dna<IStructure> parent1 = ChooseParent();
                Dna<IStructure> parent2 = ChooseParent();

                Dna<IStructure> child = parent1.Crossover(parent2);

                child.Mutate(MutationRate);

                newPopulation.Add(child);
            }

            Population = newPopulation;

            Generation++;
        }

        public void CalculateFitness()
        {
            _fitnessSum = 0;
            Dna<IStructure> best = Population[0];
            Population.ForEach(p =>
            {
                _fitnessSum += p.CalculateFitness(Population.IndexOf(p));
                best = best.Fitness < p.Fitness ? p : best;
            });

            BestFitness = best.Fitness;
            best.Genes.CopyTo(BestGenes, 0);
        }

        public float SubjectFitness(int index)
        {
            int score = 0;
            var dna = this.Population[index];
            switch (_currentEpoch)
            {
                case Epoch.First:
                    {
                        if (Rules.Find(r => r is BuildingsCountRule)
                            .IsSatisfied(BestGenes, dna.Genes, Generation, Epoch.First, Fields))
                        {
                            score++;
                            if (Rules.Find(r => r is SettlementDensityRule)
                                .IsSatisfied(BestGenes, dna.Genes, Generation, Epoch.First, Fields))
                            {
                                score++;
                            }
                            if (Rules.Find(r => r is DistanceToWaterRule)
                                .IsSatisfied(BestGenes, dna.Genes, Generation, Epoch.First, Fields))
                            {
                                score++;
                            }
                        }

                        if (score == 3)
                            SetNextEpoch();

                        break;
                    }
            }
            return score;
        }

        public Dna<IStructure> ChooseParent()
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