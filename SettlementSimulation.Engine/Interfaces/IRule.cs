using SettlementSimulation.Engine.Models;
using System.Collections.Generic;

namespace SettlementSimulation.Engine.Interfaces
{
    public interface IRule
    {
        bool IsSatisfied(IEnumerable<IStructure> prevBestGenes,
            IEnumerable<IStructure> genes,
            int generation,
            Epoch epoch,
            IEnumerable<Field> settlementFields = null);
    }
}
