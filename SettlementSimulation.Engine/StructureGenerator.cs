using SettlementSimulation.AreaGenerator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;

namespace SettlementSimulation.Engine
{
    public class StructureGenerator
    {
        private readonly SimulationEngine _engine;
        private readonly Timer _timer;
        private int _tick;

        private readonly List<int> _breakpoints;
        private readonly int _maxIterations;
        private readonly int _timeout;

        public SettlementState SettlementState { get; set; }

        public event EventHandler Finished;
        public event EventHandler Breakpoint;
        public event EventHandler NextEpoch;

        public StructureGenerator(Field[,] fields, List<Point> mainRoad,
            List<int> breakpoints, int maxIterations, int timeout)
        {
            _breakpoints = breakpoints;
            _maxIterations = maxIterations;
            _timeout = timeout;
            _engine = new SimulationEngine(100, 1, fields, mainRoad);
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
       
        private void SetUpSettlementState()
        {
            var previousSettlementState = SettlementState;
            SettlementState = new SettlementState()
            {
                CurrentGeneration = _tick,
                Time = (int) (_timer.Interval / 1000) * _tick,
                CurrentEpoch = (Epoch) (_tick / (_maxIterations / 3)),
                Structures = new List<IStructure>()
            };

            var rand = new Random();
            var initialLocation = ((Building) previousSettlementState?.Structures.First())?.Location.Point ??
                                  new Point(rand.Next(_engine.Fields.GetLength(0), _engine.Fields.GetLength(1)));

            for (int i = 0; i < _tick; i++)
            {
                var structure = _engine.GetRandomGene();
                ((Building) structure).Location = 
                    new Location(initialLocation.X+3*i,initialLocation.Y+3*i);
                SettlementState.Structures.Add(structure);
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
    }
}
