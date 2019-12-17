using System.Collections.Generic;

namespace SettlementSimulation.Engine.Models
{
    public class EpochInfo
    {
        public Dictionary<Epoch, int> DevelopmentRate = new Dictionary<Epoch, int>
        {
            {Epoch.First, 0},
            {Epoch.Second, 0},
            {Epoch.Third, 0}
        };

        public Dictionary<Epoch, double> MutationRate = new Dictionary<Epoch, double>
        {
            {Epoch.First, 0.01},
            {Epoch.Second, 0.1},
            {Epoch.Third, 0.05}
        };

        public Dictionary<Epoch, double> DurationRate = new Dictionary<Epoch, double>
        {
            {Epoch.First, 3.0/10},
            {Epoch.Second, 5/10.0},
            {Epoch.Third, 2/10.0}
        };
    }
}