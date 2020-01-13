namespace SettlementSimulation.Engine.Models
{
    public class SettlementConfiguration
    {
        public int MinRoadLength { get; set; }
        public int MaxRoadLength { get; set; }
        public int MinDistanceBetweenRoads { get; set; }
        public int InitialRoadsCount { get; set; }
        public int FirstEpochBuildings { get; set; }
        public int SecondEpochBuildings { get; set; }
        public int ThirdEpochBuildings { get; set; }
        public float FirstEpochMutationRate { get; set; }
        public float SecondEpochMutationRate { get; set; }
        public float ThirdEpochMutationRate { get; set; }
    }
}
