using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Helpers;
using SettlementSimulation.Engine.Interfaces;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

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
        private CancellationTokenSource _cancellationTokenSource;
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
        }

        public async Task Start()
        {
            _timer.Start();
            _cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() =>
            {
                while (true)
                {
                    _engine.NewGeneration();
                    if (_breakpoints.Contains(_engine.Generation))
                    {
                        SetUpSettlementState();
                        OnBreakpoint();
                    }

                    if (_tick == _maxIterations || _tick == _timeout)
                    {
                        SetUpSettlementState();
                        OnFinished();
                        _timer.Stop();
                        break;
                    }
                    Thread.Sleep(10);
                }
            }, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _timer.Stop();
            _cancellationTokenSource.Cancel();
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
                CurrentGeneration = _engine.Generation,
                Time = (int)(_timer.Interval / 1000) * _tick,
                CurrentEpoch = Epoch.First,//TODO
                Structures = _engine.BestGenes
            };

            if (previousSettlementState != null)
            {
                SettlementState.Structures.AddRange(previousSettlementState.Structures);
            }
        }
    }
}
