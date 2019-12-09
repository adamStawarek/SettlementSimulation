using SettlementSimulation.Engine.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace SettlementSimulation.Engine.Models.Roads
{
    public abstract class Road : IStructure
    {
        public abstract double Probability { get; }
        public List<Point> Points { get; set; }
    }
}