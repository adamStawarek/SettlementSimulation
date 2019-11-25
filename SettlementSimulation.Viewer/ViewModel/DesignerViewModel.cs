using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.Command;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class DesignerViewModel : INotifyPropertyChanged
    {
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
                SetTailPoints(_endY, false);
            }
        }
        public PlotModel Plot { get; }
        public LineSeries S1 { get; }
        #endregion

        #region commands
        public RelayCommand ClearBreakpointsCommand { get; } 
        #endregion

        public DesignerViewModel()
        {
            Plot = new PlotModel { Title = "Select breakpoints" };
            S1 = new LineSeries
            {
                Points = { new DataPoint(1, 0), new DataPoint(10000, 9999) },
                MarkerFill = OxyColors.SteelBlue,
                MarkerType = MarkerType.Circle,
                MarkerSize = 5
            };

            _endY = 9999;
            Plot.Series.Add(S1);
            Plot.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Minimum = 0, Maximum = 10000 });
            Plot.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 10000 });

            Plot.MouseDown += (s, e) =>
            {
                Series series = Plot.GetSeriesFromPoint(e.Position, 10);
                if (series != null)
                {
                    var point = S1.InverseTransform(e.Position);
                    S1.Points.Add(new DataPoint(point.X, point.Y));
                    S1.Points.Sort((p, p2) => p.X.CompareTo(p2.X));
                }

                Plot.InvalidatePlot(true);
            };

            ClearBreakpointsCommand=new RelayCommand(ClearBreakpoints);
        }

        private void ClearBreakpoints()
        {
            int numberOfPointsToRemove = S1.Points.Count - 2;//we leave first and last point
            S1.Points.RemoveRange(1, numberOfPointsToRemove);
            Plot.InvalidatePlot(true);
        }

        public void SetTailPoints(int val, bool isFirstPoint)
        {
            if (isFirstPoint)
            {
                S1.Points.RemoveAt(0);
                S1.Points.Add(new DataPoint(0, val));
            }
            else
            {
                var lastIndex = S1.Points.Count - 1;
                S1.Points.RemoveAt(lastIndex);
                S1.Points.Add(new DataPoint(10000, val));
            }
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
