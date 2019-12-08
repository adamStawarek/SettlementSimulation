using System;

namespace SettlementSimulation.Engine.Models
{
    public class Location
    {
        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; }
        public int Y { get; }

        public override bool Equals(object obj)
        {
            if (obj is Location l)
            {
                return l.X == X && l.Y == Y;
            }
            return false;
        }

        public double DistanceTo(Location other)
        {
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        }
    }
}