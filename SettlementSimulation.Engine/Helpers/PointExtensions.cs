using SettlementSimulation.AreaGenerator.Models;
using System;
using System.Collections.Generic;

namespace SettlementSimulation.Engine.Helpers
{
    public static class PointExtensions
    {
        public static double DistanceTo(this Point point, Point other)
        {
            return Math.Sqrt(Math.Pow(point.X - other.X, 2) + Math.Pow(point.Y - other.Y, 2));
        }

        public static List<Point> GetCircularPoints(this Point center, double radius, double angleInterval)
        {
            List<Point> points = new List<Point>();

            for (double interval = 0; interval < 2 * Math.PI; interval += angleInterval)
            {
                int X = (int)(center.X + (radius * Math.Cos(interval)));
                int Y = (int)(center.Y + (radius * Math.Sin(interval)));

                points.Add(new Point(X, Y));
            }

            return points;
        }
    }
}