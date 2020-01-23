using System.Linq;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Models.Buildings.FirstType;
using SettlementSimulation.Engine.Models.Buildings.SecondType;
using SettlementSimulation.Engine.Models.Buildings.ThirdType;
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

        public static bool IsSatisfiedBuildingCountCondition(Settlement settlement, Epoch epoch)
        {
            switch (epoch)
            {
                case Epoch.First:
                    return settlement.Buildings.Count >= FirstEpochBuildings;
                case Epoch.Second:
                    return settlement.Buildings.Count >= SecondEpochBuildings;
                case Epoch.Third:
                    return settlement.Buildings.Count >= ThirdEpochBuildings;
            }

            return false;
        }

        public static bool IsSatisfiedBuildingTypesCondition(Settlement settlement, Epoch epoch)
        {
            switch (epoch)
            {
                case Epoch.First:
                    return settlement.Buildings.Count(b => b is Market) >= 2 &&
                           settlement.Buildings.Count(b => b is Tavern) >= 5;
                case Epoch.Second:
                    return settlement.Buildings.Count(b => b is Administration) >= 1 &&
                           settlement.Buildings.Count(b => b is Church) >= 2 &&
                           settlement.Buildings.Count(b => b is School) >= 2;
                case Epoch.Third:
                    return settlement.Buildings.Count(b => b is Port) >= 1 &&
                           settlement.Buildings.Count(b => b is University) >= 1;
            }

            return false;
        }

        public static bool CanEnterNextEpoch(Settlement settlement, Epoch epoch)
        {
            return IsSatisfiedBuildingCountCondition(settlement, epoch) &&
                   IsSatisfiedBuildingTypesCondition(settlement, epoch);
        }
    }
}