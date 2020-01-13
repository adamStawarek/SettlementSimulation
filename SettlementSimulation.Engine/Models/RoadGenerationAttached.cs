using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Models
{
    public class RoadGenerationAttached
    {
        public Field[,] Fields { get; set; }
        public List<IRoad> Roads { get; set; }
        public IRoad Road { get; set; }
        public int MinDistanceBetweenRoads { get; set; }
        public int MinRoadLength { get; set; }
        public int MaxRoadLength { get; set; }
        public Point SettlementCenter { get; set; }

        public RoadGenerationAttached()
        {
            Roads = new List<IRoad>();
        }
    }
}