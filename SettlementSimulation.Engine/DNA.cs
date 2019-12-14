using System;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine
{
    public class Dna
    {
        #region
        private readonly Field[,] _fields;
        private readonly List<Point> _mainRoad;
        #endregion

        #region properties
        public IStructure[] Genes { get; set; }
        public float Fitness { get; private set; }
        #endregion

        public Dna(int size,
            Field[,] fields,
            List<Point> mainRoad,
            bool shouldInitGenes = true)
        {
            _fields = fields;
            _mainRoad = mainRoad;
            Genes = new IStructure[size];

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
                            (randomGeneBuilding==null || f.Position.DistanceTo(randomGeneBuilding.Location.Point) < 10) &&
                            f.DistanceToMainRoad + f.DistanceToWater < 400)
                .Select(p => p.Position)
                .ToArray();

            building.Location = new Location(positions[RandomProvider.Next(positions.Count())]);

            return building;
        }

        public float CalculateFitness(Epoch epoch)
        {
            int score = 0;
            switch (epoch)
            {
                case Epoch.First:
                    {
                        break;
                    }
            }

            return Fitness = score;
        }

        public Dna Crossover(Dna otherParent)
        {
            var child = new Dna(Genes.Length, _fields, _mainRoad, false);
            for (int i = 0; i < Genes.Length; i++)
            {
                child.Genes[i] = RandomProvider.NextDouble() < 0.5 ? Genes[i] : otherParent.Genes[i];
            }

            return child;
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