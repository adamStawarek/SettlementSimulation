using Newtonsoft.Json;
using SettlementSimulation.Engine.Models;
using System.IO;

namespace SettlementSimulation.Engine.Helpers
{
    public static class ConfigurationManager
    {
        private static readonly SettlementConfiguration Configuration;
        static ConfigurationManager()
        {
            Configuration = JsonConvert.DeserializeObject<SettlementConfiguration>(File.ReadAllText("settings.json"));
        }

        public static SettlementConfiguration SettlementConfiguration => Configuration;
        public static int BuildingsPerUpdate => Configuration.BuildingsPerUpdate;
        public static int RoadsPerUpdate => Configuration.RoadsPerUpdate;
        public static int MinRoadLength => Configuration.MinRoadLength;
        public static int MaxRoadLength => Configuration.MaxRoadLength;
        public static int MinDistanceBetweenRoads => Configuration.MinDistanceBetweenRoads;
        public static int InitialRoadsCount => Configuration.InitialRoadsCount;
        public static int FirstEpochBuildings => Configuration[Enumerators.Epoch.First].Buildings;
        public static int SecondEpochBuildings => Configuration[Enumerators.Epoch.Second].Buildings;
        public static int ThirdEpochBuildings => Configuration[Enumerators.Epoch.Third].Buildings;
        public static double FirstEpochProbOfWoodenBuildings => Configuration[Enumerators.Epoch.First].ProbOfWoodenBuildings;
        public static double SecondEpochProbOfWoodenBuildings => Configuration[Enumerators.Epoch.Second].ProbOfWoodenBuildings;
        public static double ThirdEpochProbOfWoodenBuildings => Configuration[Enumerators.Epoch.Third].ProbOfWoodenBuildings;
        public static double FirstEpochProbOfStoneBuildings => Configuration[Enumerators.Epoch.First].ProbOfStoneBuildings;
        public static double SecondEpochProbOfStoneBuildings => Configuration[Enumerators.Epoch.Second].ProbOfStoneBuildings;
        public static double ThirdEpochProbOfStoneBuildings => Configuration[Enumerators.Epoch.Third].ProbOfStoneBuildings;
        public static double FirstEpochProbOfBricksBuildings => Configuration[Enumerators.Epoch.First].ProbOfBricksBuildings;
        public static double SecondEpochProbOfBricksBuildings => Configuration[Enumerators.Epoch.Second].ProbOfBricksBuildings;
        public static double ThirdEpochProbOfBricksBuildings => Configuration[Enumerators.Epoch.Third].ProbOfBricksBuildings;

        public static int MaxBuildingsToAddPerIteration => Configuration.MaxBuildingsToAddPerIteration;
        public static double FloodMutationDistanceToWater => Configuration.FloodMutationDistanceToWater;
        public static double FloodMutationProbability => Configuration.FloodMutationProbability;
        public static double EarthquakeMutationProbability => Configuration.EarthquakeMutationProbability;
        public static double FireMutationProbability => Configuration.FireMutationProbability;
    }   
}
