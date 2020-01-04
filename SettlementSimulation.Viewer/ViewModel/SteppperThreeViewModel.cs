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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using SettlementSimulation.Engine.Interfaces;
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
        #endregion

        #region properties
        private List<string> _logs;
        public List<string> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                RaisePropertyChanged();
            }
        }

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
        #endregion

        public StepperThreeViewModel()
        {
            _viewModelLocator = new ViewModelLocator();

            Messenger.Default.Register<SetSettlementInfoCommand>(this, this.SetSettlementInfo);

            Logs = new List<string>();
            StructuresLegend = new Dictionary<Type, Tuple<Color, string>>();
            SpinnerVisibility = Visibility.Hidden;

            StartSimulationCommand = new RelayCommand(StartSimulation);
            StopSimulationCommand = new RelayCommand(StopSimulation);
            SetUpStructureLegend();
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

            Logs.Clear();
            SpinnerVisibility = Visibility.Visible;

            await _generator.Start();
        }

        private void StopSimulation()
        {
            _generator.Stop();

            SpinnerVisibility = Visibility.Hidden;
        }

        private void OnBreakpoint(object sender, EventArgs e)
        {
            UpdateSettlementBitmap();
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

            var roads = SettlementState.Roads.Select(s => s.Segments.Select(sg => sg.Position)).ToList();

            MarkBuildingsAndRoads(roads, buildings, originalColorMap);
            MarkPoint(SettlementState.SettlementCenter, originalColorMap, Color.Goldenrod, 4);

            var logs = new List<string>
            {
                $"Roads: {SettlementState.Roads.Count}",
                $"Buildings: {SettlementState.Roads.Sum(r => r.Buildings.Count)}",
                $"Last generated structures: " +
                $"{SettlementState.LastCreatedStructures.Aggregate("",(s1,s2)=>$"{s1.ToString()}\n{s2.ToString()}")}",
                $"Average road length: {(int)SettlementState.Roads.Average(r => r.Length)}",
                $"Min road length: {SettlementState.Roads.Min(r => r.Length)}",
                $"Max road length: {SettlementState.Roads.Max(r => r.Length)}"
            };
            _logs = new List<string>(logs);

            var allRoadPoints = roads.SelectMany(s => s).ToList();
            var upperLeft = new Point(allRoadPoints.Min(s => s.X), allRoadPoints.Min(s => s.Y));
            var bottomRight = new Point(allRoadPoints.Max(s => s.X), allRoadPoints.Max(s => s.Y));

            var trimmedBitmap = GetTrimmedBitmap(originalColorMap, upperLeft, bottomRight);
            var previewBitmap = GetPreviewBitmap(_colorMap, upperLeft, bottomRight);

            SettlementBitmap = new Bitmap(trimmedBitmap);
            PreviewBitmap = new Bitmap(previewBitmap);

            RaisePropertyChanged(nameof(Logs));
            RaisePropertyChanged(nameof(SettlementBitmap));
            RaisePropertyChanged(nameof(PreviewBitmap));
        }

        private void MarkBuildingsAndRoads(
            List<IEnumerable<Point>> roads,
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
                var roadPoints = road.ToList();
                roadPoints.ForEach(p => MarkPoint(p, originalColorMap, Color.Black));
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
