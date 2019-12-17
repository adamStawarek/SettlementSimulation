using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Rules;

namespace SettlementSimulation.Engine
{
    public class Dna
    {
        #region

        private readonly int _startDnaSize = 10;
        private readonly Field[,] _fields;
        private readonly List<Point> _mainRoad;
        private readonly IRuleDistributor _ruleDistributor;
        private readonly EpochInfo _epochInfo;
        #endregion

        #region properties
        public IStructure[] Genes { get; set; }
        public float Fitness { get; private set; }
        #endregion

        public Dna(
            Field[,] fields,
            List<Point> mainRoad,
            bool shouldInitGenes = true)
        {
            _fields = fields;
            _mainRoad = mainRoad;
            Genes = new IStructure[_startDnaSize];
            _ruleDistributor = new RuleDistributor();
            _epochInfo = new EpochInfo();

            if (!shouldInitGenes) return;
            for (var i = 0; i < Genes.Length; i++)
            {
                Genes[i] = GetRandomGene(Epoch.First);
            }
        }

        public IStructure GetRandomGene(Epoch epoch)
        {
            //TODO check whether to generate building or road

            var building = Building.GetRandom(epoch);

            var takenPositions = Genes
                .Where(g => g is Building)
                .Cast<Building>()
                .Select(s => s.Location.Point)
                .ToArray();

            var randomGeneBuilding = (Building)Genes
                .OrderBy(g => Guid.NewGuid())
                .FirstOrDefault(g => g is Building);

            var positions = _fields.ToList()
                .Where(f => f.InSettlement &&
                            !takenPositions.Contains(f.Position) &&
                            (randomGeneBuilding == null || f.Position.DistanceTo(randomGeneBuilding.Location.Point) < 10) &&
                            f.DistanceToMainRoad + f.DistanceToWater < 400)
                .Select(p => p.Position)
                .ToArray();

            building.Location = new Location(positions[RandomProvider.Next(positions.Count())]);

            return building;
        }

        public float CalculateFitness(Epoch epoch, int generation)
        {
            int score = 0;

            var executionInfo = new RuleExecutionInfo() { Epoch = epoch, Fields = _fields, Genes = this.Genes };
            if (_ruleDistributor.GetRule<BuildingsCountRule>().IsSatisfied(executionInfo))
            {
                score++;
            }

            if (score == 1 && _ruleDistributor.GetRule<MarketsRule>().IsSatisfied(executionInfo))
            {
                score++;
            }

            if (score == 2 && _ruleDistributor.GetRule<MarketsRule>().IsSatisfied(executionInfo))
            {
                score++;
            }

            return Fitness = score;
        }

        public Dna Crossover(Dna otherParent, Epoch epoch)
        {
            //TODO join this parts of the dna's that don't overlap

            var bestDna = this.Fitness > otherParent.Fitness ? this : otherParent;
            var fitness = (int)bestDna.Fitness;
            var child=new Dna(bestDna._fields, bestDna._mainRoad, false);

            if (fitness == 0)
            {
                var structure = GetRandomGene(epoch);
                child.Genes = new IStructure[child.Genes.Length + 1];
                bestDna.Genes.CopyTo(child.Genes,0);
                child.Genes[child.Genes.Count()-1] = structure;
            }

            if (fitness == 1)
            {
                ShuffleBuildingsLocations();
            }
          

            return child;
        }

        public void ShuffleBuildingsLocations()
        {
            var takenPositions = Genes
                .Where(g => g is Building)
                .Cast<Building>()
                .Select(s => s.Location.Point)
                .ToArray();

            var randomGeneBuilding = (Building)Genes
                .OrderBy(g => Guid.NewGuid())
                .FirstOrDefault(g => g is Building);

            var positions = _fields.ToList()
                .Where(f => f.InSettlement &&
                            !takenPositions.Contains(f.Position) &&
                            (randomGeneBuilding == null || f.Position.DistanceTo(randomGeneBuilding.Location.Point) < 10) &&
                            f.DistanceToMainRoad + f.DistanceToWater < 400)
                .Select(p => p.Position)
                .ToList();

            foreach (var gene in Genes.Where(g=>g is Building).Cast<Building>())
            {
                var locationIndex = RandomProvider.Next(positions.Count());
                gene.Location = new Location(positions[locationIndex]);
                positions.RemoveAt(locationIndex);
            }
        }

        public void Mutate(Epoch epoch, float mutationRate = 0.01F)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (RandomProvider.NextDouble() < mutationRate)
                {
                    Genes[i] = GetRandomGene(epoch);
                }
            }
        }
    }
}