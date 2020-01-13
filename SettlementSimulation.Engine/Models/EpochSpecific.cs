using SettlementSimulation.Engine.Enumerators;
using static SettlementSimulation.Engine.Helpers.ConfigurationManager;

namespace SettlementSimulation.Engine.Models
{
    public static class EpochSpecific
    {
        public static int GetBuildingsCount(Epoch epoch)
        {
            switch (epoch)
            {
                case Epoch.First:
                    return FirstEpochBuildings;
                case Epoch.Second:
                    return SecondEpochBuildings;
                case Epoch.Third:
                    return ThirdEpochBuildings;
            }

            return -1;
        }

        public static double GetMutationRate(Epoch epoch)
        {
            switch (epoch)
            {
                case Epoch.First:
                    return FirstEpochMutationRate;
                case Epoch.Second:
                    return SecondEpochMutationRate;
                case Epoch.Third:
                    return ThirdEpochMutationRate;
            }

            return -1;
        }
    }
}