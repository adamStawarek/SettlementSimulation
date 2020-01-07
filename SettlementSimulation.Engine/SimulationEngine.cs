using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;

namespace SettlementSimulation.Engine
{
    public class SimulationEngine
    {
        #region private fields
        private readonly Stack<Epoch> _allEpochs;
        #endregion

        #region properties
        public Epoch CurrentEpoch { get; set; }
        public int Generation { get; set; }
        public Field[,] Fields { get; }
        public List<Point> MainRoad { get; }
        public IEnumerable<ISettlementStructure> LastStructuresCreated { get; set; }
        public List<IRoad> BestGenes => BestDna.Genes;
        public Dna BestDna { get; set; }
        #endregion

        public SimulationEngine(
            Field[,] fields,
            List<Point> mainRoad)
        {
            Fields = fields;
            MainRoad = mainRoad;
            Generation = 1;
            BestDna = new Dna(fields, mainRoad);

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
            CalculateFitness();

            var generatedStructures = BestDna.CreateNewDnaStructure(CurrentEpoch, SetNextEpoch);

            LastStructuresCreated = null;
            if (generatedStructures.NewRoads.Any())
            {
                generatedStructures.NewRoads
                    .ForEach(r => BestDna.AddRoad(r));
                LastStructuresCreated = generatedStructures.NewRoads.ToList();
            }

            if (generatedStructures.NewBuildings.Any())
            {
                generatedStructures.NewBuildings
                    .ForEach(b =>
                    {
                        BestDna.AddBuildingToRoad(generatedStructures.RoadToAttachNewBuildings, b);
                        LastStructuresCreated = generatedStructures.NewBuildings.ToList();
                    });
            }

            Generation++;
        }

        public void CalculateFitness()
        {
            //TODO
        }

    }
}