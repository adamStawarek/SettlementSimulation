using SettlementSimulation.Engine.Enumerators;

namespace SettlementSimulation.Engine.Models
{
    public static class EpochSpecific
    {
        public const int BuildingsFirstEpoch = 500;
        public const int BuildingsSecondEpoch = 5000;
        public const int BuildingsThirdEpoch = 5000;

        public static int GetBuildingsCount(Epoch epoch)
        {
            switch (epoch)
            {
                case Epoch.First:
                    return BuildingsFirstEpoch;
                case Epoch.Second:
                    return BuildingsSecondEpoch;
                case Epoch.Third:
                    return BuildingsThirdEpoch;
            }

            return -1;
        }

        public static double GetMutationRate(Epoch epoch)
        {
            switch (epoch)
            {
                case Epoch.First:
                    return 0.01;
                case Epoch.Second:
                    return 0.1;
                case Epoch.Third:
                    return 0.05;
            }

            return -1;
        }
    }
}