using FastBitmapLib;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SettlementSimulation.AreaGenerator.Models;
using SettlementSimulation.Engine;
using SettlementSimulation.Engine.Models;
using SettlementSimulation.Engine.Models.Buildings;
using SettlementSimulation.Viewer.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using SettlementSimulation.Engine.Enumerators;
using SettlementSimulation.Engine.Interfaces;
using Color = System.Drawing.Color;
using Point = SettlementSimulation.AreaGenerator.Models.Point;

namespace SettlementSimulation.Viewer.ViewModel
{
    public class StepperThreeViewModel : ViewModelBase
    {
        #region fields
        private Bitmap _colorMap;
        private Bitmap _heightMap;
        private SettlementInfo _settlementInfo;
        private StructureGenerator _generator;
        private readonly ViewModelLocator _viewModelLocator;
        private readonly List<int> _buildingsPerIteration;
        private readonly string _screenshootPath;
        #endregion

        #region properties
        private ObservableCollection<string> _updateUpdateLogs;
        public ObservableCollection<string> UpdateLogs
        {
            get => _updateUpdateLogs;
            set
            {
                _updateUpdateLogs = value;
                RaisePropertyChanged();
            }
        }

        private List<string> _generalStateLogs;
        public List<string> GeneralStateLogs
        {
            get => _generalStateLogs;
            set
            {
                _generalStateLogs = value;
                RaisePropertyChanged();
            }
        }

        public SeriesCollection SettlementGraphValues { get; set; }

        public Dictionary<Type, Tuple<Color, string>> StructuresLegend { get; set; }

        private Bitmap _settlementBitmap;
        public Bitmap SettlementBitmap
        {
            get => _settlementBitmap;
            set
            {
                _settlementBitmap = value;
                RaisePropertyChanged();
            }
        }

        private Bitmap _previewBitmap;
        public Bitmap PreviewBitmap
        {
            get => _previewBitmap;
            set
            {
                _previewBitmap = value;
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

        private SettlementState _settlementState;
        public SettlementState SettlementState
        {
            get => _settlementState;
            set
            {
                _settlementState = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region commands
        public RelayCommand StartSimulationCommand { get; }
        public RelayCommand StopSimulationCommand { get; }
        public RelayCommand<object> TakeScreenshotCommand { get; set; }
        #endregion

        public StepperThreeViewModel()
        {
            _viewModelLocator = new ViewModelLocator();
            _buildingsPerIteration = new List<int>();
            _screenshootPath = $"{DateTime.Now.ToString("s").Replace(":", "-")}";
            Directory.CreateDirectory(_screenshootPath);

            Messenger.Default.Register<SetSettlementInfoCommand>(this, this.SetSettlementInfo);
            SettlementGraphValues = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<double>(),
                    Title="Buildings per iteration"
                },
            };

            GeneralStateLogs = new List<string>();
            UpdateLogs = new ObservableCollection<string>();
            StructuresLegend = new Dictionary<Type, Tuple<Color, string>>();
            SpinnerVisibility = Visibility.Hidden;

            StartSimulationCommand = new RelayCommand(StartSimulation);
            StopSimulationCommand = new RelayCommand(StopSimulation);
            TakeScreenshotCommand = new RelayCommand<object>(TakeScreenshot);
            SetUpStructureLegend();
        }

        private void TakeScreenshot(object obj)
        {
            var elements = (object[])obj;

            foreach (var uiElem in elements)
            {
                try
                {
                    var appendix = elements.ToList().IndexOf(uiElem) == 0 ? "map" : "graph";
                    var source = uiElem as UIElement;
                    double renderHeight, renderWidth;

                    var height = renderHeight = source.RenderSize.Height;
                    var width = renderWidth = source.RenderSize.Width;

                    //Specification for target bitmap like width/height pixel etc.
                    RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
                    //creates Visual Brush of UIElement
                    VisualBrush visualBrush = new VisualBrush(source);

                    DrawingVisual drawingVisual = new DrawingVisual();
                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        //draws image of element
                        drawingContext.DrawRectangle(visualBrush, null,
                            new Rect(new System.Windows.Point(0, 0), new System.Windows.Point(width, height)));
                    }
                    //renders image
                    renderTarget.Render(drawingVisual);

                    //PNG encoder for creating PNG file
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                    using (FileStream stream = new FileStream($"{_screenshootPath}/{SettlementState.CurrentIteration + "_" + appendix}.png", FileMode.Create, FileAccess.Write))
                    {
                        encoder.Save(stream);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

        }

        private void SetSettlementInfo(SetSettlementInfoCommand obj)
        {
            _settlementInfo = obj.SettlementInfo;
            _heightMap = obj.HeightMap;
            _colorMap = obj.ColorMap;
            SettlementBitmap = new Bitmap(_colorMap);

            foreach (var point in _settlementInfo.MainRoad)
            {
                MarkPoint(point, SettlementBitmap, Color.Red, 3);
            }
        }

        private async void StartSimulation()
        {
            var maxIterations = _viewModelLocator.Designer.EndY;
            var breakpoints = _viewModelLocator.Designer.Breakpoints;

            _generator = new StructureGeneratorBuilder()
                .WithMaxIterations(maxIterations)
                .WithBreakpoints(breakpoints)
                .WithFields(_settlementInfo.Fields)
                .WithMainRoad(_settlementInfo.MainRoad)
                .Build();

            _generator.Breakpoint += OnBreakpoint;
            _generator.NextEpoch += OnNextEpoch;
            _generator.Finished += OnFinished;
            _generator.Initialized += OnInitialized;

            UpdateLogs.Clear();
            SpinnerVisibility = Visibility.Visible;

            await _generator.Start();
        }

        private void StopSimulation()
        {
            _generator.Stop();

            SpinnerVisibility = Visibility.Hidden;
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            UpdateSettlementBitmap();
        }

        private void OnBreakpoint(object sender, EventArgs e)
        {
            UpdateSettlementBitmap();
            var buildingsCount = SettlementState.Roads.Sum(r => r.Buildings.Count);
            _buildingsPerIteration.Add(buildingsCount);

            if (SettlementState.CurrentIteration % 10 == 0)
            {
                SettlementGraphValues.First().Values.Add((double)_buildingsPerIteration.Last());
            }

            RaisePropertyChanged(nameof(SettlementGraphValues));
        }

        private void OnNextEpoch(object sender, EventArgs e)
        {
            UpdateSettlementBitmap();
        }

        private void OnFinished(object sender, EventArgs e)
        {
            UpdateSettlementBitmap();

            MessageBox.Show($"Execution finished");

            SpinnerVisibility = Visibility.Hidden;
        }

        private void UpdateSettlementBitmap()
        {
            SettlementState = _generator.SettlementState;

            var originalColorMap = new Bitmap(_colorMap);

            var buildings = SettlementState.Roads
                .SelectMany(s => s.Segments.SelectMany(seg => seg.Buildings)).ToList();

            var roads = SettlementState.Roads.ToList();

            MarkBuildingsAndRoads(roads, buildings, originalColorMap);
            MarkPoint(SettlementState.SettlementCenter, originalColorMap, Color.Goldenrod, 4);

            SetUpLogs();

            var allRoadPoints = roads.SelectMany(s => s.Segments.Select(sg => sg.Position)).ToList();
            var upperLeft = new Point(allRoadPoints.Min(s => s.X), allRoadPoints.Min(s => s.Y));
            var bottomRight = new Point(allRoadPoints.Max(s => s.X), allRoadPoints.Max(s => s.Y));

            var trimmedBitmap = GetTrimmedBitmap(originalColorMap, upperLeft, bottomRight);
            var previewBitmap = GetPreviewBitmap(_colorMap, upperLeft, bottomRight);

            SettlementBitmap = new Bitmap(trimmedBitmap);
            PreviewBitmap = new Bitmap(previewBitmap);

            RaisePropertyChanged(nameof(UpdateLogs));
            RaisePropertyChanged(nameof(SettlementBitmap));
            RaisePropertyChanged(nameof(PreviewBitmap));
        }

        private void SetUpLogs()
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
           {
               int maxLogs = 10;
               if (UpdateLogs.Count > maxLogs)
               {
                   UpdateLogs.RemoveAt(0);
               }

               switch (SettlementState.LastSettlementUpdate.UpdateType)
               {
                   case UpdateType.NewRoads:
                       UpdateLogs.Add($"{SettlementState.LastSettlementUpdate.UpdateType}" +
                                      $"({SettlementState.LastSettlementUpdate.NewRoads.Count})");
                       break;
                   case UpdateType.NewBuildings:
                       UpdateLogs.Add($"{SettlementState.LastSettlementUpdate.UpdateType}" +
                                      $"({SettlementState.LastSettlementUpdate.NewBuildings.Count})");
                       break;
                   case UpdateType.NewTypes:
                       UpdateLogs.Add($"{SettlementState.LastSettlementUpdate.UpdateType}" +
                                      $"({SettlementState.LastSettlementUpdate.UpdatedBuildings.Count})");
                       break;
               }
           }));

            var logs = new List<string>
            {
                $"Epoch: {SettlementState.CurrentEpoch}",
                $"Iteration: {SettlementState.CurrentIteration}",
                $"Roads: {SettlementState.Roads.Count}",
                $"Buildings: {SettlementState.Roads.Sum(r => r.Buildings.Count)}",
                $"Average road length: {(int) SettlementState.Roads.Average(r => r.Length)}",
                $"Min road length: {SettlementState.Roads.Min(r => r.Length)}",
                $"Max road length: {SettlementState.Roads.Max(r => r.Length)}"
            };

            var groups = SettlementState.Roads
                .SelectMany(r => r.Buildings)
                .GroupBy(building => building.GetType());

            logs.AddRange(groups.Select(typeBuildings => $"{typeBuildings.Key.Name}: {typeBuildings.Count()}"));

            GeneralStateLogs = new List<string>(logs);
            RaisePropertyChanged(nameof(GeneralStateLogs));
        }

        private void MarkBuildingsAndRoads(
            List<IRoad> roads,
            List<IBuilding> buildings,
            Bitmap originalColorMap)
        {
            foreach (var building in buildings)
            {
                var point = building.Position;
                MarkPoint(point, originalColorMap, StructuresLegend[building.GetType()].Item1);
            }

            foreach (var road in roads)
            {
                var roadPoints = road.Segments.Select(sg => sg.Position).ToList();
                roadPoints.ForEach(p =>
                    MarkPoint(p, originalColorMap, road.Type.Equals(RoadType.Unpaved) ? Color.Black : Color.DarkGray));
            }

            _settlementInfo.MainRoad.ForEach(p => MarkPoint(p, originalColorMap, Color.Red));
        }

        private Bitmap GetPreviewBitmap(
            Bitmap colorMap,
            Point upperLeft,
            Point bottomRight)
        {
            var bitmap = new Bitmap(colorMap);

            using (var fastBitmap = bitmap.FastLock())
            {
                for (int i = upperLeft.X; i < bottomRight.X; i++)
                {
                    for (int j = upperLeft.Y; j < bottomRight.Y; j++)
                    {
                        var color = fastBitmap.GetPixel(i, j);
                        fastBitmap.SetPixel(i, j, Color.FromArgb(255, color.G, color.B));
                    }
                }
            }

            return bitmap;
        }

        private Bitmap GetTrimmedBitmap(
            Bitmap bitmap,
            Point upperLeft,
            Point bottomRight)
        {
            var minX = upperLeft.X;
            var minY = upperLeft.Y;
            var maxX = bottomRight.X;
            var maxY = bottomRight.Y;

            var offset = 10;
            var width = maxX - minX + 2 * offset;
            var height = maxY - minY + 2 * offset;
            var tmpBitmap = new Bitmap(width, height);

            using (var fastBitmap = bitmap.FastLock())
            {
                for (int i = minX - offset, n = 0; i < maxX + offset; i++, n++)
                {
                    for (int j = minY - offset, m = 0; j < maxY + offset; j++, m++)
                    {
                        if (i < 0 || j < 0 || i >= fastBitmap.Width || j >= fastBitmap.Height) continue;
                        var color = fastBitmap.GetPixel(i, j);
                        tmpBitmap.SetPixel(n, m, color);
                    }
                }
            }

            return tmpBitmap;
        }

        private void SetUpStructureLegend()
        {
            var structures = Assembly.Load("SettlementSimulation.Engine")
                .GetTypes()
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Building)))
                .ToList();

            for (int i = 0; i < structures.Count(); i++)
            {
                StructuresLegend.Add(structures[i], new Tuple<Color, string>(GetRandColor(i), structures[i].Name));
            }
        }

        private void MarkPoint(Point point, Bitmap bitmap, Color color)
        {
            using (var fastBitmap = bitmap.FastLock())
            {
                fastBitmap.SetPixel(point.X, point.Y, color);
            }
        }

        private void MarkPoint(Point point, Bitmap bitmap, Color color, int offset)
        {
            using (var fastBitmap = bitmap.FastLock())
            {
                for (int i = -offset; i < offset; i++)
                {
                    for (int j = -offset; j < offset; j++)
                    {
                        if (point.X + i < 0 || point.X + i >= bitmap.Width || point.Y + j < 0 || point.Y + j >= bitmap.Height)
                            continue;
                        fastBitmap.SetPixel(point.X + i, point.Y + j, color);
                    }
                }
            }
        }

        private Color GetRandColor(int index)
        {
            var colors = new List<Color>
            {
                Color.FromArgb(128, 0, 0),
                Color.FromArgb(170, 110, 40),
                Color.FromArgb(128, 128, 120),
                Color.FromArgb(0, 128, 128),
                Color.FromArgb(0, 0, 128),
                Color.FromArgb(230, 25, 75),
                Color.FromArgb(245, 130, 48),
                Color.FromArgb(110, 145, 160),
                Color.FromArgb(220, 180, 75),
                Color.FromArgb(0, 140, 240),
                Color.FromArgb(0, 130, 200),
                Color.FromArgb(145, 30, 180),
                Color.FromArgb(240, 50, 230),
                Color.FromArgb(128, 128, 128),
                Color.FromArgb(250, 190, 190),
                Color.FromArgb(255, 215, 180),
                Color.FromArgb(170, 255, 195),
                Color.FromArgb(230, 190, 255),
                Color.FromArgb(230, 25, 80),
                Color.FromArgb(60, 190, 200),
                Color.FromArgb(0, 60, 60),
            };

            return colors[index % colors.Count()];
        }
    }
}
