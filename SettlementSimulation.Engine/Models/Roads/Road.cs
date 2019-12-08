using System.Collections.Generic;
using System.Drawing;
using SettlementSimulation.Engine.Interfaces;

namespace SettlementSimulation.Engine.Models.Roads
{
    public abstract class Road : IStructure
    {
        public abstract double Probability { get; }
        public List<Point> Points { get; set; }
    }
}