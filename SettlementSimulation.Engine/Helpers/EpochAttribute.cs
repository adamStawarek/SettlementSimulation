using SettlementSimulation.Engine.Models;
using System;

namespace SettlementSimulation.Engine.Helpers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EpochAttribute : Attribute
    {
        public Epoch Epoch { get; }

        public EpochAttribute(Epoch epoch)
        {
            Epoch = epoch;
        }
    }
}