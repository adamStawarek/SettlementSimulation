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


    }   
}
