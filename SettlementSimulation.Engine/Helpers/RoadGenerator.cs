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
                new Position(model.Start.X, model.Start.Y), new Position(model.End.X,model.End.Y),
                MovementPatterns.LateralOnly);

            return positions.Select(p => new Point(p.X, p.Y));
        }

        public IEnumerable<Point> GenerateAttached(RoadGenerationAttached model)
        {
            //var road = model.Road;

            //var segment = road.Segments[RandomProvider.Next(0, road.Segments.Count)];

            //var positions = new List<Point>
            //{
            //    new Point(segment.Position.X - 1, segment.Position.Y),
            //    new Point(segment.Position.X + 1, segment.Position.Y),
            //    new Point(segment.Position.X, segment.Position.Y - 1),
            //    new Point(segment.Position.X, segment.Position.Y + 1),
            //};
            //positions.RemoveAll(p => road.BlockedCells.Contains(p) ||
            //                         road.AttachedRoads.Any(a => a.DistanceTo(p) <= 2)); //TODO
            //if (!positions.Any())
            //    throw new Exception("No places for building available");

            //var start = positions[RandomProvider.Next(0, positions.Count)];

            //var largestRoadLength = Genes.Max(g => g.Length);
            //var shortestRoadLength = Genes.Min(g => g.Length);
            return null;
        }
    }
}