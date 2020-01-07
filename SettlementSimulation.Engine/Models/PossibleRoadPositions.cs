using SettlementSimulation.Engine.Interfaces;
using System.Collections.Generic;

namespace SettlementSimulation.Engine.Models
{
    public class PossibleRoadPositions
    {
        public PossibleRoadPositions(IEnumerable<IRoad> roads)
        {
            Roads = new List<IRoad>(roads);
            MinDistanceBetweenRoads = 15;
        }

        public List<IRoad> Roads { get; set; }
        public int MinDistanceBetweenRoads { get; set; }
    }
}
