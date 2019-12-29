using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.Engine.Models
{
    public class RoadGenerationTwoPoints
    {
        public Field[,] Fields { get; set; }
        public IEnumerable<Point> BlockedCells { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }

        public RoadGenerationTwoPoints()
        {
            BlockedCells = new List<Point>();
        }
    }
}