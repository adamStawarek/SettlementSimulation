using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using System.Collections.Generic;

namespace SettlementSimulation.Engine.Models
{
    public class PossibleBuildingPositions
    {
        public PossibleBuildingPositions(IEnumerable<IRoad> roads, Field[,] fields)
        {
            Roads = new List<IRoad>(roads);
            Fields = fields;
        }

        public List<IRoad> Roads { get; set; }
        public Field[,] Fields { get; set; }
    }
}
