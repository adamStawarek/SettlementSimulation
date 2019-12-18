using System;
using SettlementSimulation.Engine.Interfaces;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.Engine.Models.Roads
{
    public abstract class Road : IStructure
    {
        public abstract double Probability { get; }
        public List<Point> Points { get; set; }

        public override string ToString()
        {
            return $"{nameof(Type)}: {this.GetType().Name} " +
                   $"{nameof(Points)}: \n" +
                   $"{Points.Aggregate("", (l1, l2) => l1 + "\n\t " + l2, r => r.ToString())} ";
        }
    }
}