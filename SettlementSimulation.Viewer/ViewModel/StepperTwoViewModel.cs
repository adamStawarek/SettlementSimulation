using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LiveCharts;
using LiveCharts.Wpf;
using SettlementSimulation.AreaGenerator;
using SettlementSimulation.Viewer.Commands;
using SettlementSimulation.Viewer.ViewModel.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using Color = System.Drawing.Color;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class StepperTwoViewModel : ViewModelBase
    {
        #region private fields
        private Bitmap _originalHeightMap;
        #endregion

        #region properties
        private int _minHeight;
        public int MinHeight
        {
            get => _minHeight;
            set
            {
                _minHeight = value;
                RaisePropertyChanged();
                //FindPotentialArea();
            }
        }

        private int _maxHeight;
        public int MaxHeight
        {
            get => _maxHeight;
            set
            {
                _maxHeight = value;
                RaisePropertyChanged();
                //FindPotentialArea();
            }
        }

        private Bitmap _heightMap;
        public Bitmap HeightMap
        {
            get => _heightMap;
            set
            {
                _heightMap = value;
                RaisePropertyChanged();
            }
        }

        private Bitmap _colorMap;
        public Bitmap ColorMap
        {
            get => _colorMap;
            set
            {
                _colorMap = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _spinnerVisibility;
        public Visibility SpinnerVisibility
        {
            get => _spinnerVisibility;
            set
            {
                _spinnerVisibility = value;
                RaisePropertyChanged();
            }
        }

        private string _rgbVal;
        public string RgbVal
        {
            get => _rgbVal;
            set
            {
                _rgbVal = value;
                RaisePropertyChanged("RgbVal");
            }
        }

        public SeriesCollection HistogramValues { get; set; }

        private bool _canContinue;
        public bool CanContinue
        {
            get => _canContinue;
            set
            {
                _canContinue = value;
                RaisePropertyChanged();
            }
        }

        private double _colorMapOpacity;
        public double ColorMapOpacity
        {
            get => _colorMapOpacity;
            set
            {
                _colorMapOpacity = value; 
                RaisePropertyChanged();
            }
        }
        #endregion

        #region commands
        public RelayCommand FindAreaCommand { get; }
        public RelayCommand<object> OpenPopupCommand { get; }
        public RelayCommand ResetCommand { get; }
        #endregion

        public StepperTwoViewModel()
        {
            Messenger.Default.Register<SetHeightMapCommand>(this, this.SetHeightMap);
            Messenger.Default.Register<SetColorMapCommand>(this, this.SetColorMap);

            _colorMapOpacity = 0.9;
            _minHeight = 0;
            _maxHeight = 70;
            _spinnerVisibility = Visibility.Hidden;

            HistogramValues = new SeriesCollection
            {
                new ColumnSeries {Values = new ChartValues<double>{}},
            };

            FindAreaCommand = new RelayCommand(FindPotentialArea);
            OpenPopupCommand = new RelayCommand<object>(SetCurrentPixelValuesToRgbBox);
            ResetCommand=new RelayCommand(Reset);
        }

        private void Reset()
        {
            this.HeightMap=new Bitmap(_originalHeightMap);
            RaisePropertyChanged(nameof(HeightMap));
        }

        private void SetHeightMap(SetHeightMapCommand command)
        {
            CanContinue = false;
            HeightMap = new Bitmap(command.HeightMap);
            _originalHeightMap = new Bitmap(command.HeightMap);
            SetGraphs();
        }

        private void SetColorMap(SetColorMapCommand command)
        {
            CanContinue = false;
            ColorMap = new Bitmap(command.ColorMap);
        }

        public void SetGraphs()
        {
            HistogramValues.First().Values.Clear();
            var histogramValues = new List<double>(SetHistogram());
            foreach (double value in histogramValues)
            {
                HistogramValues.First().Values.Add(value);
            }
        }

        public IEnumerable<double> SetHistogram()
        {
            Dictionary<int, double> histogramRgb = new Dictionary<int, double>();
            int scale = 1;
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                if (!histogramRgb.ContainsKey(i / scale))
                    histogramRgb.Add(i / scale, 0);
            }

            for (int y = 0; y < _originalHeightMap.Height; y++)
            {
                for (int x = 0; x < _originalHeightMap.Width; x++)
                {
                    var p = _originalHeightMap.GetPixel(x, y);
                    histogramRgb[p.R / scale]++;
                }
            }

            return histogramRgb.Values.ToList();
        }

        private async void FindPotentialArea()
        {
            SpinnerVisibility = Visibility.Visible;
            HeightMap = new Bitmap(_originalHeightMap);
            var (points, bitmap) = await new SettlementBuilder()
                .WithColorMap(_colorMap)
                .WithHeightMap(_heightMap)
                .WithHeightRange(_minHeight, _maxHeight)
                .BuildAsync();
            _heightMap = new Bitmap(bitmap);
            RaisePropertyChanged(nameof(HeightMap));
            SpinnerVisibility = Visibility.Hidden;
            CanContinue = true;
        }

        #region helper functions
        private void SetCurrentPixelValuesToRgbBox(object obj)
        {
            var color = ColorUnderCursor.Get();
            POINT p;
            ColorUnderCursor.GetCursorPos(out p);
            RgbVal = $"Intensity: {GetGreyscale(color)}";
        }

        private byte GetGreyscale(Color color)
        {
            return (byte)((color.R + color.B + color.G) / 3);
        }
        #endregion
    }
}