using Newtonsoft.Json;
using SettlementSimulation.Engine.Models;
using System.IO;

namespace SettlementSimulation.Engine.Helpers
{
    public static class ConfigurationManager
    {
        private static SettlementConfiguration Configuration;
        static ConfigurationManager()
        {
            Configuration = JsonConvert.DeserializeObject<SettlementConfiguration>(File.ReadAllText("settings.json"));
        }

        public static int MinRoadLength => Configuration.MinRoadLength;
        public static int MaxRoadLength => Configuration.MaxRoadLength;
        public static int MinDistanceBetweenRoads => Configuration.MinDistanceBetweenRoads;
        public static int InitialRoadsCount => Configuration.InitialRoadsCount;
        public static int FirstEpochBuildings => Configuration.FirstEpochBuildings;
        public static int SecondEpochBuildings => Configuration.SecondEpochBuildings;
        public static int ThirdEpochBuildings => Configuration.ThirdEpochBuildings;
        public static float FirstEpochMutationRate => Configuration.FirstEpochMutationRate;
        public static float SecondEpochMutationRate => Configuration.SecondEpochMutationRate;
        public static float ThirdEpochMutationRate => Configuration.ThirdEpochMutationRate;


    }   
}
