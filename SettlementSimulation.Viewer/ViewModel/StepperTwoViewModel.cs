using FastBitmapLib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LiveCharts;
using LiveCharts.Wpf;
using SettlementSimulation.AreaGenerator;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Viewer.Commands;
using SettlementSimulation.Viewer.Helpers;
using SettlementSimulation.Viewer.ViewModel.Helpers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
                ConfigurationManagerHelper.SetSettings("MinHeight", _minHeight.ToString());
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
                ConfigurationManagerHelper.SetSettings("MaxHeight", _maxHeight.ToString());
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
        public RelayCommand<object> ChangeColorMapOpacityCommand { get; set; }
        #endregion

        public StepperTwoViewModel()
        {
            Messenger.Default.Register<SetHeightMapCommand>(this, this.SetHeightMap);
            Messenger.Default.Register<SetColorMapCommand>(this, this.SetColorMap);

            _colorMapOpacity = 0.9;
            int.TryParse(ConfigurationManagerHelper.GetSettings("MinHeight"), out _minHeight);
            int.TryParse(ConfigurationManagerHelper.GetSettings("MaxHeight"), out _maxHeight);
            _spinnerVisibility = Visibility.Hidden;

            HistogramValues = new SeriesCollection
            {
                new ColumnSeries {Values = new ChartValues<double>{}},
            };

            FindAreaCommand = new RelayCommand(FindPotentialArea);
            OpenPopupCommand = new RelayCommand<object>(SetCurrentPixelValuesToRgbBox);
            ResetCommand = new RelayCommand(Reset);
            ChangeColorMapOpacityCommand = new RelayCommand<object>(ChangeColorMapOpacity);
        }

        private void ChangeColorMapOpacity(object obj)
        {
            var args = obj as MouseWheelEventArgs;
            ColorMapOpacity += args.Delta < 0 ? -0.05 : 0.05;
            SetCurrentPixelValuesToRgbBox(obj);
        }

        private void Reset()
        {
            this.HeightMap = new Bitmap(_originalHeightMap);
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
            var settlementInfo = await new SettlementBuilder()
                .WithColorMap(this.CreatePixelMatrix(_colorMap))
                .WithHeightMap(this.CreatePixelMatrix(_heightMap))
                .WithHeightRange(_minHeight, _maxHeight)
                .BuildAsync();
            _heightMap = this.CreateBitmap(settlementInfo.PreviewBitmap);
            RaisePropertyChanged(nameof(HeightMap));
            SpinnerVisibility = Visibility.Hidden;
            CanContinue = true;

            Messenger.Default.Send(new SetSettlementInfoCommand()
            {
                SettlementInfo = settlementInfo,
                HeightMap = new Bitmap(_originalHeightMap),
                ColorMap = new Bitmap(_colorMap)
            });
        }

        private Bitmap CreateBitmap(Pixel[,] pixels)
        {
            var bitmap = new Bitmap(pixels.GetLength(0), pixels.GetLength(1));

            using (var fastBitmap = bitmap.FastLock())
            {
                for (int i = 0; i < pixels.GetLength(0); i++)
                {
                    for (int j = 0; j < pixels.GetLength(1); j++)
                    {
                        var pixel = pixels[i, j];
                        fastBitmap.SetPixel(i, j, Color.FromArgb(pixel.R, pixel.G, pixel.B));
                    }
                }
            }

            return bitmap;

        }

        private Pixel[,] CreatePixelMatrix(Bitmap bitmap)
        {
            var pixels = new Pixel[bitmap.Width, bitmap.Height];
            using (var fastBitmap = bitmap.FastLock())
            {
                for (int i = 0; i < fastBitmap.Width; i++)
                {
                    for (int j = 0; j < fastBitmap.Height; j++)
                    {
                        var pixel = fastBitmap.GetPixel(i, j);
                        pixels[i, j] = new Pixel(pixel.R, pixel.G, pixel.B);
                    }
                }
            }
            return pixels;
        }

        private void SetCurrentPixelValuesToRgbBox(object obj)
        {
            var color = ColorUnderCursor.Get();
            POINT p;
            ColorUnderCursor.GetCursorPos(out p);
            RgbVal = $"rgb({color.B},{color.G},{color.R})";
        }
    }
}