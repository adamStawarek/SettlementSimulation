using System;

namespace SettlementSimulation.AreaGenerator.Models
{
    public struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public double DistanceTo(Point other)
        {
            return Math.Sqrt(Math.Pow(this.X - other.X, 2) + Math.Pow(this.Y - other.Y, 2));
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}