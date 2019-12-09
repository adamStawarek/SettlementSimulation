using SettlementSimulation.AreaGenerator.Models;
using System.Collections.Generic;
using System.Drawing;

namespace SettlementSimulation.Engine
{
    public class StructureGeneratorBuilder
    {
        private Field[,] _fields;
        private List<Point> _mainRoad;
        private List<int> _breakpoints;
        private int _maxIterations = 10000;
        private int _timeout = 10 * 60 * 1000;

        public StructureGeneratorBuilder WithFields(Field[,] fields)
        {
            this._fields = fields;
            return this;
        }

        public StructureGeneratorBuilder WithMainRoad(IEnumerable<Point> mainRoad)
        {
            this._mainRoad = new List<Point>(mainRoad);
            return this;
        }

        public StructureGeneratorBuilder WithBreakpoints(IEnumerable<int> breakpoints)
        {
            this._breakpoints = new List<int>(breakpoints);
            return this;
        }

        public StructureGeneratorBuilder WithMaxIterations(int maxIterations)
        {
            this._maxIterations = maxIterations;
            return this;
        }

        public StructureGeneratorBuilder WithTimeout(int timeout)
        {
            this._timeout = timeout;
            return this;
        }

        public StructureGenerator Build()
        {
            var generator = new StructureGenerator(_fields, _mainRoad, _breakpoints,
                _maxIterations, _timeout);

            return generator;
        }
    }
}