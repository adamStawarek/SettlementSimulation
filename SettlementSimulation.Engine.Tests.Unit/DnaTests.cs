using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace SettlementSimulation.Engine.Tests.Unit
{
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public class DnaTests
    {
        [Test]
        public void InitializeGenes_Does_Not_Throw_Exception()
        {
            Assert.DoesNotThrow(()=>new Dna(null, null, shouldInitGenes: true));
        }
    }
}