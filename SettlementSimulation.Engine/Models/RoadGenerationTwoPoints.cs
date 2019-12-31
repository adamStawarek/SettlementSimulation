using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.Engine.Models
{
    public class RoadGenerationTwoPoints
    {
        public Field[,] Fields { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
    }
}