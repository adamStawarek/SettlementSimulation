using System.Collections.Generic;
using System.Threading.Tasks;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IRoadGenerator 
    {
        IEnumerable<Point> Generate(RoadGenerationInfo roadInfo);
    }
}