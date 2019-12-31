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
            _stopWatch = new Stopwatch();
            _engine = new SimulationEngine(1, fields, mainRoad);
        }

        public async Task Start()
        {
            _stopWatch.Start();
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

                    if (_engine.Generation == _maxIterations || 
                        _stopWatch.ElapsedMilliseconds == _timeout)
                    {
                        SetUpSettlementState();
                        OnFinished();
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
            SettlementState = new SettlementState()
            {
                CurrentGeneration = _engine.Generation,
                Time = (int)_stopWatch.ElapsedMilliseconds/1000,
                CurrentEpoch = Epoch.First,//TODO
                Structures = _engine.BestGenes
            };
        }
    }
}
