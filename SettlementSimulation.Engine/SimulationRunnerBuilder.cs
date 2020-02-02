using SettlementSimulation.AreaGenerator.Models;
using System.Collections.Generic;

namespace SettlementSimulation.Engine
{
    public class SimulationRunnerBuilder
    {
        private Field[,] _fields;
        private List<Point> _mainRoad;
        private List<int> _breakpoints;
        private int _maxIterations = 10000;
        private int _timeout = 10 * 60 * 1000;
        private int? _uniformBreakpointStep;

        public SimulationRunnerBuilder WithFields(Field[,] fields)
        {
            this._fields = fields;
            return this;
        }

        public SimulationRunnerBuilder WithMainRoad(IEnumerable<Point> mainRoad)
        {
            this._mainRoad = new List<Point>(mainRoad);
            return this;
        }

        public SimulationRunnerBuilder WithBreakpoints(IEnumerable<int> breakpoints)
        {
            this._breakpoints = new List<int>(breakpoints);
            return this;
        }

        public SimulationRunnerBuilder WithBreakpointStep(int step)
        {
            _uniformBreakpointStep = step;
            return this;
        }

        public SimulationRunnerBuilder WithMaxIterations(int maxIterations)
        {
            this._maxIterations = maxIterations;
            return this;
        }

        public SimulationRunnerBuilder WithTimeout(int timeout)
        {
            this._timeout = timeout;
            return this;
        }

        public SimulationRunner Build()
        {
            if (_uniformBreakpointStep.HasValue)
            {
                _breakpoints = new List<int>();
                for (int i = 0; i < _maxIterations; i += _uniformBreakpointStep.Value)
                {
                    _breakpoints.Add(i);
                }
            }

            var generator = new SimulationRunner(_fields, _mainRoad, _breakpoints,
                _maxIterations, _timeout);

            return generator;
        }
    }
}