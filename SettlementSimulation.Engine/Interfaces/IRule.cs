using System.Collections.Generic;
using SettlementSimulation.Engine.Models;

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
