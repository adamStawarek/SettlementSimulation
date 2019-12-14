using System;
using System.Drawing;
using SettlementSimulation.Engine.Helpers;

namespace SettlementSimulation.Engine.Models
{
    public class Location
    {
        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Location(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public int X { get; }
        public int Y { get; }

        public Point Point => new Point(X, Y);

        public double DistanceTo(Location other)
        {
            return this.Point.DistanceTo(other.Point);
        }

        public override bool Equals(object obj)
        {
            if (obj is Location l)
            {
                return l.X == X && l.Y == Y;
            }
            return false;
        }
    }
}