using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace SettlementSimulation.Engine
{
    public class StructureGenerator
    {
        #region fields
        private readonly List<int> _breakpoints;
        private readonly int _maxIterations;
        private readonly int _timeout;
        private readonly SimulationEngine _engine;
        private readonly Timer _timer;
        private int _tick;
        #endregion

        #region events
        public event EventHandler Finished;
        public event EventHandler Breakpoint;
        public event EventHandler NextEpoch;
        #endregion

        #region properties
        public SettlementState SettlementState { get; private set; }
        #endregion

        public StructureGenerator(Field[,] fields, List<Point> mainRoad,
            List<int> breakpoints, int maxIterations, int timeout)
        {
            _breakpoints = breakpoints;
            _maxIterations = maxIterations;
            _timeout = timeout;
            _engine = new SimulationEngine(1, fields, mainRoad);
            _timer = new Timer { Interval = 100 };
            _timer.Elapsed += OnTick;
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {
            _tick++;

            if (_tick == _maxIterations / 3 || _tick == 2 * _maxIterations / 3)
            {
                _engine.SetNextEpoch();
                SetUpSettlementState();
                OnNextEpoch();
            }

            if (_breakpoints.Contains(_tick))
            {
                SetUpSettlementState();
                OnBreakpoint();
            }

            if (_tick == _maxIterations || _tick == _timeout)
            {
                SetUpSettlementState();
                OnFinished();
                _timer.Stop();
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void OnFinished()
        {
            var handler = Finished;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public void OnBreakpoint()
        {
            var handler = Breakpoint;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public void OnNextEpoch()
        {
            var handler = NextEpoch;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void SetUpSettlementState()
        {
            var previousSettlementState = SettlementState;
            SettlementState = new SettlementState()
            {
                CurrentGeneration = _tick,
                Time = (int)(_timer.Interval / 1000) * _tick,
                CurrentEpoch = Epoch.First,//TODO
                Structures = new List<IBuilding>()
            };

            if (previousSettlementState != null)
            {
                SettlementState.Structures.AddRange(previousSettlementState.Structures);
            }

            var rand = new Random();
            var takenPositions = previousSettlementState?.Structures.Cast<Building>()
                .Select(s => s.Position).ToArray();
            var positions = _engine.Fields.ToList()
                .Where(f => f.InSettlement &&
                            (takenPositions == null || !takenPositions.Contains(f.Position)) &&
                            f.DistanceToMainRoad + f.DistanceToWater < 300)
                .Select(p => p.Position)
                .ToArray();

            for (int i = 0; i < Math.Log(_tick - previousSettlementState?.Structures?.Count() ?? 0); i++)
            {
                var building = Building.GetRandom(SettlementState.CurrentEpoch);
                building.Position= positions[rand.Next(positions.Count())];
                SettlementState.Structures.Add(building);
            }
        }
    }
}
