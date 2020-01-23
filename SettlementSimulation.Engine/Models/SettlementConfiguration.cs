using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.Engine.Enumerators;

namespace SettlementSimulation.Engine.Models
{
    public class SettlementConfiguration
    {
        public int MinRoadLength { get; set; }
        public int MaxRoadLength { get; set; }
        public int MinDistanceBetweenRoads { get; set; }
        public int InitialRoadsCount { get; set; }
        public int BuildingsPerUpdate { get; set; }
        public int RoadsPerUpdate { get; set; }
        public List<EpochInfo> EpochInfos { get; set; }

        public EpochInfo this[Epoch epoch]
        {
            get { return EpochInfos.Single(s => s.Epoch.Equals(epoch)); }
        }

        public class EpochInfo
        {
            public Epoch Epoch { get; set; }
            public int Buildings { get; set; }
            public float ProbNewRoad { get; set; }
            public float ProbNewBuildings { get; set; }
            public float ProbUpdate { get; set; }
        }
    }
}
