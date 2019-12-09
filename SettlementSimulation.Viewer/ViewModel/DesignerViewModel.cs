using GalaSoft.MvvmLight.Command;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class DesignerViewModel : INotifyPropertyChanged
    {
        #region fields
        private const int MaxGenerations = 10000;
        #endregion

        #region properties
        private int _endY;
        public int EndY
        {
            get => _endY;
            set
            {
                if (value == _endY) return;
                _endY = value;
                OnPropertyChanged();
                SetTailPoint(_endY);
            }
        }
        public PlotModel Plot { get; }
        public LineSeries S1 { get; }
        public List<int> Breakpoints { get; set; }
        #endregion

        #region commands
        public RelayCommand ClearBreakpointsCommand { get; }
        #endregion

        public DesignerViewModel()
        {
            Breakpoints = new List<int>();
            Plot = new PlotModel { Title = "Select breakpoints" };
            S1 = new LineSeries
            {
                Points = { new DataPoint(1, 0), new DataPoint(MaxGenerations, MaxGenerations - 1) },
                MarkerFill = OxyColors.SteelBlue,
                MarkerType = MarkerType.Circle,
                MarkerSize = 5
            };

            _endY = 9999;
            Plot.Series.Add(S1);
            Plot.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Minimum = 0, Maximum = MaxGenerations });
            Plot.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Minimum = 0, Maximum = MaxGenerations });

            Plot.MouseDown += (s, e) =>
            {
                Series series = Plot.GetSeriesFromPoint(e.Position, 10);
                if (series != null)
                {
                    var point = S1.InverseTransform(e.Position);
                    var x = point.X;
                    var y = (_endY / (double)MaxGenerations) * point.X;
                    S1.Points.Add(new DataPoint(x, y));
                    S1.Points.Sort((p, p2) => p.X.CompareTo(p2.X));
                    Breakpoints.Add((int)y);
                }

                Plot.InvalidatePlot(true);
            };

            ClearBreakpointsCommand = new RelayCommand(ClearBreakpoints);
        }

        private void ClearBreakpoints()
        {
            int numberOfPointsToRemove = S1.Points.Count - 2;
            S1.Points.RemoveRange(1, numberOfPointsToRemove);
            Plot.InvalidatePlot(true);
            Breakpoints.Clear();
        }

        public void SetTailPoint(int val)
        {
            ClearBreakpoints();

            var lastIndex = S1.Points.Count - 1;
            S1.Points.RemoveAt(lastIndex);

            S1.Points.Add(new DataPoint(MaxGenerations, val));

            S1.Points.Sort((p, p2) => p.X.CompareTo(p2.X));
            Plot.InvalidatePlot(true);
        }

        #region property changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
