using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Models
{
    public class RoadGenerationInfo
    {
        public Field[,] Fields { get; set; }
        public IEnumerable<IBuilding> Structures { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
    }
}