using SettlementSimulation.Viewer.Commands;
using SettlementSimulation.Viewer.ViewModel.Helpers;
using FastBitmapLib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class StepperTwoViewModel : ViewModelBase
    {
        #region private fields
        private Bitmap _originalHeightMap;
        private readonly Color _areaColor;
        private Dictionary<Point, byte> _maxAreaPoints;
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

        public ObservableCollection<ObservableKeyValuePair<string, string>> AdditionalInfo { get; set; }

        public SeriesCollection HistogramValues { get; set; }
        
        #endregion

        #region commands
        public RelayCommand FindAreaCommand { get; }
        public RelayCommand<object> OpenPopupCommand { get; }
        #endregion

        public StepperTwoViewModel()
        {
            Messenger.Default.Register<SetHeightMapCommand>(this, this.SetHeightMap);

            _areaColor = Color.Red;
            _minHeight = 0;
            _maxHeight = 70;
            _maxAreaPoints = new Dictionary<Point, byte>();
            _spinnerVisibility = Visibility.Hidden;

            HistogramValues = new SeriesCollection
            {
                new ColumnSeries {Values = new ChartValues<double>{}},
            };

            AdditionalInfo = new ObservableCollection<ObservableKeyValuePair<string, string>>
            {
                 new ObservableKeyValuePair<string, string>("Width",""),
                new ObservableKeyValuePair<string, string>("Height",""),
                new ObservableKeyValuePair<string, string>("Execution time(ms)","")
            };

            FindAreaCommand = new RelayCommand(FindPotentialArea);
            OpenPopupCommand = new RelayCommand<object>(SetCurrentPixelValuesToRgbBox);
        }

        private void SetHeightMap(SetHeightMapCommand command)
        {
            AdditionalInfo.First(f => f.Key == "Width").Value = command.HeightMap.Width.ToString();
            AdditionalInfo.First(f => f.Key == "Height").Value = command.HeightMap.Height.ToString();
            RaisePropertyChanged(nameof(AdditionalInfo));
            HeightMap = new Bitmap(command.HeightMap);
            _originalHeightMap = new Bitmap(command.HeightMap);
            SetGraphs();
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
            _maxAreaPoints.Clear();
            var potentialArePoints = GetPotentialArePoints();
            while (potentialArePoints.Count > 0 && potentialArePoints.Count > _maxAreaPoints.Count)
            {
                _heightMap = await ApplyFloodFillAsync(potentialArePoints.First(), potentialArePoints);
            }
            RaisePropertyChanged(nameof(HeightMap));
            SpinnerVisibility = Visibility.Hidden;
        }

        private List<Point> GetPotentialArePoints()
        {
            int offsetWidth = 10;
            int offsetHeight = 10;
            var potentialArePoints = new List<Point>();
            for (int y = offsetHeight; y < _heightMap.Height - offsetHeight; y++)
            {
                for (int x = offsetWidth; x < _heightMap.Width - offsetWidth; x++)
                {
                    var pixel = _heightMap.GetPixel(x, y);
                    byte blue = pixel.R;
                    byte green = pixel.G;
                    byte red = pixel.B;
                    byte intensity = GetGreyscale(red, green, blue);
                    if (intensity >= _minHeight && intensity <= _maxHeight)
                        potentialArePoints.Add(new Point(x, y));
                }
            }

            return potentialArePoints;
        }

        private async Task<Bitmap> ApplyFloodFillAsync(Point startPoint, List<Point> potentialAreaPoints)
        {
            var bitmap = new Bitmap(_heightMap);
            var fastBitmap = new FastBitmap(bitmap);
            return await Task.Run(delegate
            {
                var timeStarted = DateTime.Now;

                
                Stack<Point> pixels = new Stack<Point>();
                fastBitmap.Lock();
                var targetColor = fastBitmap.GetPixel(startPoint.X, startPoint.Y);
                fastBitmap.Unlock();
                GetGreyscale(targetColor.R, targetColor.G, targetColor.B);
                pixels.Push(startPoint);
                var marked = new Dictionary<Point, byte>();
                while (pixels.Count > 0)
                {
                    Point a = pixels.Pop();
                    if (a.X < fastBitmap.Width && a.X > -1 && a.Y < fastBitmap.Height && a.Y > -1)
                    {
                        fastBitmap.Lock();
                        var pixelColor = fastBitmap.GetPixel(a.X, a.Y);
                        byte intensity = GetGreyscale(pixelColor.R, pixelColor.G, pixelColor.B);
                        //when pixel intensity is significantly different that target intensity, set this pixel to black
                        if (intensity >= _minHeight && intensity <= _maxHeight && !marked.ContainsKey(a))
                        {
                            fastBitmap.SetPixel(a.X, a.Y, _areaColor);
                            marked.Add(a, intensity);
                            pixels.Push(new Point(a.X - 1, a.Y));
                            pixels.Push(new Point(a.X + 1, a.Y));
                            pixels.Push(new Point(a.X, a.Y - 1));
                            pixels.Push(new Point(a.X, a.Y + 1));
                        }
                        fastBitmap.Unlock();
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    potentialAreaPoints.RemoveAll(p => marked.ContainsKey(p));
                    //remove pixels that are contained in area from potential area pixels
                });

                fastBitmap.Lock();
                ////when marked size is less than previous picked area we change its color back
                if (marked.Count < _maxAreaPoints.Count)
                    marked.ToList().ForEach(p =>
                        fastBitmap.SetPixel(p.Key.X, p.Key.Y,
                            Color.FromArgb(p.Value, p.Value, p.Value)));
                else
                {
                    _maxAreaPoints.ToList().ForEach(p =>
                        fastBitmap.SetPixel(p.Key.X, p.Key.Y,
                            Color.FromArgb(p.Value, p.Value, p.Value)));
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _maxAreaPoints.Clear();
                        _maxAreaPoints = marked;
                    });
                }
                fastBitmap.Unlock();


                var timeEnded = DateTime.Now;

                Application.Current.Dispatcher.Invoke(() =>
                    {
                        AdditionalInfo.First(f => f.Key == "Execution time(ms)").Value = (timeEnded - timeStarted).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
                    });

                return bitmap;
            });
        }

        #region helper functions
        private void SetCurrentPixelValuesToRgbBox(object obj)
        {
            var color = ColorUnderCursor.Get();
            POINT p;
            ColorUnderCursor.GetCursorPos(out p);
            RgbVal = $"Intensity: {GetGreyscale(color)}";
        }

        private byte GetGreyscale(byte r, byte g, byte b)
        {
            return (byte)((r + b + g) / 3);
        }

        private byte GetGreyscale(Color color)
        {
            return (byte)((color.R + color.B + color.G) / 3);
        }
        #endregion
    }
}