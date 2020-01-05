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
    public class RoadPointsGenerator : IRoadGenerator
    {
        public IEnumerable<Point> Generate(RoadGenerationTwoPoints model)
        {
            var grid = new Grid(model.Fields.GetLength(0), model.Fields.GetLength(1), 1.0f);

            for (int i = 0; i < model.Fields.GetLength(0); i++)
            {
                for (int j = 0; j < model.Fields.GetLength(1); j++)
                {
                    if (!model.Fields[i, j].InSettlement ||
                        (model.Fields[i, j].IsBlocked.HasValue && model.Fields[i, j].IsBlocked.Value))
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

        public IEnumerable<Point> GenerateStraight(RoadGenerationTwoPoints model)
        {
            var (minX, maxX) = model.Start.X < model.End.X
                ? (model.Start.X, model.End.X)
                : (model.End.X, model.Start.X);

            var (minY, maxY) = model.Start.Y < model.End.Y
                ? (model.Start.Y, model.End.Y)
                : (model.End.Y, model.Start.Y);

            var positions = new List<Point>();
            if (model.Start.X.Equals(model.End.X))
            {
                for (int i = minY; i <= maxY; i++)
                {
                    positions.Add(new Point(model.Start.X, i));
                }
            }
            else
            {
                for (int i = minX; i <= maxX; i++)
                {
                    positions.Add(new Point(i, model.Start.Y));
                }
            }

            return positions.Any(p => p.X < 0 ||
                                      p.X >= model.Fields.GetLength(0) ||
                                      p.Y < 0 ||
                                      p.Y >= model.Fields.GetLength(1)) ?
                new List<Point>() : positions;
        }

        public IEnumerable<Point> GenerateAttached(RoadGenerationAttached model)
        {
            var roads = new List<IRoad>(model.Roads);
            var possiblePositions = model.Road.GetPossiblePositionsToAttachRoad(roads, model.MinDistanceBetweenRoads);
            possiblePositions.RemoveAll(p => p.X <= 0 ||
                                             p.Y < 0 ||
                                             p.X >= model.Fields.GetLength(0) ||
                                             p.Y >= model.Fields.GetLength(1));
            if (!possiblePositions.Any())
                return new List<Point>();

            Point roadStart = possiblePositions[RandomProvider.Next(possiblePositions.Count)];
            var minRoadLength = model.MinRoadLength;
            var maxRoadLength = model.MaxRoadLength;

            var roadLength = RandomProvider.Next(minRoadLength, maxRoadLength);

            var segment = model.Road.Segments.Single(s => s.Position.X == roadStart.X ||
                                                          s.Position.Y == roadStart.Y);

            Point roadEnd;
            Direction direction;

            if (segment.Position.X.Equals(roadStart.X))//vertical road
            {
                if (segment.Position.Y > roadStart.Y)
                {
                    roadEnd = new Point(segment.Position.X, roadStart.Y - roadLength);
                    direction = Direction.Down;
                }
                else
                {
                    roadEnd = new Point(segment.Position.X, roadStart.Y + roadLength);
                    direction = Direction.Up;
                }

                if (roadEnd.Y < 0)
                    roadEnd.Y = 0;
                if (roadEnd.Y >= model.Fields.GetLength(1))
                    roadEnd.Y = model.Fields.GetLength(1) - 1;
            }
            else//horizontal road
            {
                if (segment.Position.X <= roadStart.X)
                {
                    roadEnd = new Point(segment.Position.X + roadLength, roadStart.Y);
                    direction = Direction.Right;
                }
                else
                {
                    roadEnd = new Point(segment.Position.X - roadLength, roadStart.Y);
                    direction = Direction.Left;
                }
                if (roadEnd.X < 0)
                    roadEnd.X = 0;
                if (roadEnd.X >= model.Fields.GetLength(0))
                    roadEnd.X = model.Fields.GetLength(0) - 1;
            }

            var roadPoints = this.GenerateStraight(new RoadGenerationTwoPoints()
            {
                Start = roadStart,
                End = roadEnd,
                Fields = model.Fields
            }).ToList();

            var intersectPoints = roads
                .SelectMany(r => r.Segments.Select(s => s.Position))
                .Intersect(roadPoints)
                .ToList();

            if (!intersectPoints.Any()) return roadPoints;

            Point? selectedPoint = null;
            switch (direction)
            {
                case Direction.Up:
                    {
                        selectedPoint = intersectPoints
                            .OrderBy(p => p.Y).Cast<Point?>().FirstOrDefault(p => p.Value.DistanceTo(roadStart) >= minRoadLength);
                        break;
                    }
                case Direction.Down:
                    {
                        selectedPoint = intersectPoints
                            .OrderByDescending(p => p.Y).Cast<Point?>().FirstOrDefault(p => p.Value.DistanceTo(roadStart) >= minRoadLength);
                        break;
                    }
                case Direction.Right:
                    {
                        selectedPoint = intersectPoints
                            .OrderBy(p => p.X).Cast<Point?>().FirstOrDefault(p => p.Value.DistanceTo(roadStart) >= minRoadLength);
                        break;
                    }
                case Direction.Left:
                    {
                        selectedPoint = intersectPoints
                            .OrderByDescending(p => p.X).Cast<Point?>().FirstOrDefault(p => p.Value.DistanceTo(roadStart) >= minRoadLength);
                        break;
                    }
            }
            if (!selectedPoint.HasValue)
                return new List<Point>();

            roadPoints = roadPoints.Take(roadPoints.IndexOf(selectedPoint.Value)).ToList();

            roadPoints.RemoveAll(r => model.Fields[r.X, r.Y].IsBlocked.HasValue &&
                                      model.Fields[r.X, r.Y].IsBlocked.Value);

            return roadPoints;
        }
    }
}