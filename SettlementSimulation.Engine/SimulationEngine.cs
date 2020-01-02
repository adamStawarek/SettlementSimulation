using System;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using System.Collections.Generic;
using System.Linq;

namespace SettlementSimulation.Engine
{
    public class SimulationEngine
    {
        #region private fields
        private float _fitnessSum;
        private readonly Stack<Epoch> _allEpochs;
        #endregion

        #region properties
        public ISettlementStructure LastStructureCreated { get; set; }
        public List<Dna> Population { get; set; }
        public Epoch CurrentEpoch { get; set; }
        public int Generation { get; set; }
        public Field[,] Fields { get; }
        public List<Point> MainRoad { get; }
        public List<IRoad> BestGenes => 
            Population.OrderByDescending(p => p.Fitness).FirstOrDefault()?.Genes;
        #endregion

        public SimulationEngine(
            int populationSize,
            Field[,] fields,
            List<Point> mainRoad)
        {
            Fields = fields;
            MainRoad = mainRoad;
            Generation = 1;
            Population = new List<Dna>(populationSize);
            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new Dna(fields, mainRoad));
            }

            _allEpochs = new Stack<Epoch>();
            _allEpochs.Push(Epoch.Third);
            _allEpochs.Push(Epoch.Second);
            _allEpochs.Push(Epoch.First);
            SetNextEpoch();
        }

        public void SetNextEpoch()
        {
            CurrentEpoch = _allEpochs.Pop();
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

                Dna child = parent1.Crossover(parent2, CurrentEpoch);

                child.Mutate(CurrentEpoch);

                this.LastStructureCreated = child.AddNewSettlementStructure(CurrentEpoch, SetNextEpoch);

                newPopulation.Add(child);
            }

            Population = newPopulation;

            Generation++;
        }

        public void CalculateFitness()
        {
            //TODO
        }

        public Dna ChooseParent()
        {
            //TODO
            return Population.First();
        }
    }
}