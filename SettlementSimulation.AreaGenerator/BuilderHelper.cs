using System;
using System.Collections.Generic;
using System.Linq;
using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.AreaGenerator
{
    public class BuilderHelper
    {
        public double DistanceTo(Point point, Point other)
        {
            return Math.Sqrt(Math.Pow(point.X - other.X, 2) + Math.Pow(point.Y - other.Y, 2));
        }

        public IEnumerable<Point> GetBoundaryPoints(List<Point> points)
        {
            var boundaryPoints = points.Where(p1 => points.Count(p2 => (p2.X == p1.X && p2.Y == p1.Y - 1) ||
                                                                       (p2.X == p1.X && p2.Y == p1.Y + 1) ||
                                                                       (p2.X == p1.X - 1 && p2.Y == p1.Y) ||
                                                                       (p2.X == p1.X + 1 && p2.Y == p1.Y))
                                                    < 4);

            return boundaryPoints;
        }

        public IEnumerable<Point> GetBoundaryPoints(int[,] points)
        {
            var boundaryPoints = new List<Point>();
            for (int i = 0; i < points.GetLength(0); i++)
            {
                for (int j = 0; j < points.GetLength(1); j++)
                {
                    if (points[i, j] != 1) continue;
                    if (i == 0 || j == 0 || i == points.GetLength(0) - 1 || j == points.GetLength(1) - 1 ||
                        !(points[i - 1, j - 1] == 1 &&
                        points[i - 1, j] == 1 &&
                        points[i - 1, j + 1] == 1 &&
                        points[i, j - 1] == 1 &&
                        points[i, j + 1] == 1 &&
                        points[i + 1, j - 1] == 1 &&
                        points[i + 1, j] == 1 &&
                        points[i + 1, j + 1] == 1))
                    {
                        boundaryPoints.Add(new Point(j, i));
                    }
                }
            }

            return boundaryPoints;
        }

    }
}
