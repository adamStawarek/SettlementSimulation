using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoyT.AStar;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;

namespace SettlementSimulation.Engine.Helpers
{
    public class RoadGenerator : IRoadGenerator
    {
        public IEnumerable<Point> Generate(RoadGenerationTwoPoints model)
        {
            var grid = new Grid(model.Fields.GetLength(0), model.Fields.GetLength(1), 1.0f);

            for (int i = 0; i < model.Fields.GetLength(0); i++)
            {
                for (int j = 0; j < model.Fields.GetLength(1); j++)
                {
                    if (!model.Fields[i, j].InSettlement ||
                        model.BlockedCells.Any(s => (int)s.DistanceTo(new Point(i, j)) == 0))
                    {
                        grid.BlockCell(new Position(i, j));
                    }
                }
            }

            var positions = grid.GetPath(
                new Position(model.Start.X, model.Start.Y), new Position(model.End.X, model.End.Y),
                MovementPatterns.LateralOnly);

            return positions.Select(p => new Point(p.X, p.Y));
        }

        public IEnumerable<Point> GenerateAttached(RoadGenerationAttached model)
        {
            var possiblePositions = model.Road.GetPossiblePositionsToAttachRoad(model.MinDistanceBetweenRoads);
            if (!possiblePositions.Any()) return null;

            var roadStart = possiblePositions[RandomProvider.Next(0, possiblePositions.Count)];
            var minRoadLength = model.Roads.Min(r => r.Length); //TODO - random endPoint between (minRL,maxRL)
            var maxRoadLength = model.Roads.Max(r => r.Length);

            var segment = model.Road.Segments.Single(s => s.Position.X == roadStart.X ||
                                                          s.Position.Y == roadStart.Y);

            Point roadEnd = new Point(-1, -1);
            if (segment.Position.X.Equals(roadStart.X))//vertical road
            {
                roadEnd = segment.Position.Y > roadStart.Y ?
                    new Point(segment.Position.X, roadStart.Y - maxRoadLength) :
                    new Point(segment.Position.X, roadStart.Y + maxRoadLength);
            }
            else//horizontal road
            {
                roadEnd = segment.Position.X > roadStart.X ?
                    new Point(segment.Position.X - maxRoadLength, roadStart.Y) :
                    new Point(segment.Position.X + maxRoadLength, roadStart.Y);
            }

            var roadPoints = this.Generate(new RoadGenerationTwoPoints()
            {
                Start = roadStart,
                End = roadEnd,
                Fields = model.Fields,
                BlockedCells = model.BlockedCells
            }).ToList();

            //check whether there is cross with other road - TODO optimize

            Point? intersectPoint = model.Roads //TODO - can be multiple intersect points
                .SelectMany(r => r.Segments.Select(s => s.Position))
                .Intersect(roadPoints)
                .Cast<Point?>()
                .FirstOrDefault();

            return intersectPoint != null ? 
                roadPoints.Take(roadPoints.IndexOf((Point)intersectPoint)) : 
                roadPoints;
        }
    }
}