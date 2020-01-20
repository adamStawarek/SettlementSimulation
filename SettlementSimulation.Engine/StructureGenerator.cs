using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly Stopwatch _stopWatch;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;
        #endregion

        #region events
        public event EventHandler Finished;
        public event EventHandler Breakpoint;
        public event EventHandler NextEpoch;
        public event EventHandler Initialized;
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
            _stopWatch = new Stopwatch();
            _engine = new SimulationEngine(fields, mainRoad);
        }

        public async Task Start()
        {
            _isRunning = true;
            _stopWatch.Start();
            _cancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() =>
            {
                while (_isRunning)
                {
                    var previousEpoch = _engine.CurrentEpoch;

                    _engine.NewGeneration();
                    if (_breakpoints.Contains(_engine.Generation))
                    {
                        SetUpSettlementState();
                        OnBreakpoint();
                    }

                    if (_engine.CurrentEpoch != previousEpoch)
                    {
                        SetUpSettlementState();
                        OnNextEpoch();
                    }

                    if (_engine.Generation == _maxIterations ||
                        _stopWatch.ElapsedMilliseconds == _timeout)
                    {
                        SetUpSettlementState();
                        OnFinished();
                        _isRunning = false;
                        _cancellationTokenSource.Cancel();
                        _stopWatch.Stop();
                        break;
                    }
                    Thread.Sleep(10);
                }
            }, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _stopWatch.Stop();
            _cancellationTokenSource.Cancel();
            _isRunning = false;
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

        public void OnInitialized()
        {
            var handler = Initialized;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void SetUpSettlementState()
        {
            SettlementState = new SettlementState()
            {
                MainRoad = new Road(_engine.MainRoad),
                CurrentGeneration = _engine.Generation,
                Time = (int)_stopWatch.ElapsedMilliseconds / 1000,
                CurrentEpoch = _engine.CurrentEpoch,
                Roads = _engine.Settlement.Genes,
                LastSettlementUpdate = _engine.LastSettlementUpdate,
                SettlementCenter = _engine.Settlement.SettlementCenter
            };
        }
    }
}
