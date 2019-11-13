using SettlementSimulation.Engine.Models;
using System.Collections.Generic;
using static SettlementSimulation.Engine.ReflectionHelper;

namespace SettlementSimulation.Engine
{
    public class SimulationEngine : BaseGeneticEngine<IStructure>
    {
        private Epoch _currentEpoch;
        private readonly Stack<Epoch> _allEpochs;

        public SimulationEngine(int populationSize, int dnaSize, float mutationRate = 0.01F) : base(populationSize,
            dnaSize, mutationRate)
        {
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

        public override IStructure GetRandomGene()
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

            var diceRoll = RandomProvider.NextDouble();
            double cumulative = 0.0;
            foreach (var building in buildings)
            {
                cumulative += building.Probability;
                if (diceRoll < cumulative)
                {
                    return building;
                }
            }

            return new EmptyArea();
        }

        public override float SubjectFitness(int index)
        {
            float score = 0;
            //Dna<Building> dna = this.Population[index];
            return score;
        }
    }
}