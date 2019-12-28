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
        public IEnumerable<Point> Generate(RoadGenerationInfo roadInfo)
        {
            var grid = new Grid(roadInfo.Fields.GetLength(0), roadInfo.Fields.GetLength(1), 1.0f);

            for (int i = 0; i < roadInfo.Fields.GetLength(0); i++)
            {
                for (int j = 0; j < roadInfo.Fields.GetLength(1); j++)
                {
                    if (!roadInfo.Fields[i, j].InSettlement ||
                        roadInfo.Structures.Any(s => (int)s.Position.DistanceTo(new Point(i, j)) == 0))
                    {
                        grid.BlockCell(new Position(i, j));
                    }
                }
            }

            var positions = grid.GetPath(
                new Position(roadInfo.Start.X, roadInfo.Start.Y), new Position(roadInfo.End.X,roadInfo.End.Y),
                MovementPatterns.LateralOnly);

            return positions.Select(p => new Point(p.X, p.Y));
        }
    }
}