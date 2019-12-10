using FastBitmapLib;
using RoyT.AStar;
using SettlementSimulation.AreaGenerator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SettlementSimulation.AreaGenerator
{
    public class SettlementBuilder
    {
        private Bitmap _heightMap;
        private Bitmap _colorMap;
        private int _maxHeight;
        private int _minHeight;

        public SettlementBuilder WithHeightMap(Bitmap bitmap)
        {
            _heightMap = bitmap;
            return this;
        }

        public SettlementBuilder WithColorMap(Bitmap bitmap)
        {
            _colorMap = bitmap;
            return this;
        }

        public SettlementBuilder WithHeightRange(int min, int max)
        {
            _minHeight = min;
            _maxHeight = max;
            return this;
        }

        public async Task<SettlementInfo> BuildAsync()
        {
            #region find water aquens
            var colorMap = new Bitmap(_colorMap);
            var waterAreasBoundaryFunc = new Func<Color, bool>(color => color.B >= byte.MaxValue - 5 &&
                                                                       color.R <= byte.MinValue + 5 &&
                                                                       color.G >= 50 && color.G <= 120);
            var waterAreas = new List<IEnumerable<Point>>();
            var potentialWaterPoints =
                GetPixels(colorMap, waterAreasBoundaryFunc)
                    .ToList();

            while (potentialWaterPoints.Count > 0)
            {
                var area = await ApplyFloodFillAsync(
                    colorMap,
                    potentialWaterPoints.First(),
                    waterAreasBoundaryFunc);
                potentialWaterPoints.RemoveAll(p => area.Contains(p));
                waterAreas.Add(area);
            }
            var maxWaterArea = waterAreas.Max(w => w.Count());
            waterAreas.RemoveAll(w => w.Count() < 0.4 * maxWaterArea);
            #endregion

            #region find settlement areas
            var previewBitmap = new Bitmap(_heightMap);
            var settlementAreaBoundaryFunc = new Func<Color, bool>(color => color.G >= _minHeight && color.G <= _maxHeight);
            var areas = new List<IEnumerable<Point>>();
            var potentialAreaPoints = GetPixels(previewBitmap, settlementAreaBoundaryFunc)
                .ToList();

            while (potentialAreaPoints.Count > 0)
            {
                var area = await ApplyFloodFillAsync(
                    previewBitmap,
                    potentialAreaPoints.First(),
                    settlementAreaBoundaryFunc);

                potentialAreaPoints.RemoveAll(p => area.Contains(p));
                areas.Add(area);
            }

            var selectedArea = areas.OrderByDescending(a => a.Count()).First().ToList();
            #endregion

            var builderHelper = new BuilderHelper();
            var waterMatrix = new int[colorMap.Height, colorMap.Width];
            foreach (var point in waterAreas.SelectMany(w => w))
            {
                waterMatrix[point.Y, point.X] = 1;
            }

            var boundaryPoints = builderHelper.GetBoundaryPoints(waterMatrix).OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            var nStep = boundaryPoints.Count() / (15 * waterAreas.Count);
            var waterAreaBoundaryPoints = boundaryPoints.Where((x, i) => i % nStep == 0).ToList();

            var fields = selectedArea.Select(a => new
            {
                Point = a,
                DistanceToWater = waterAreaBoundaryPoints.Min(w => builderHelper.DistanceTo(w, a))
            }).OrderBy(f => f.DistanceToWater).ToList();

            #region mark settlement area and water aquens on bitmap

            fields.Take((int)(fields.Count * 0.2)).ToList()
                .ForEach(p => MarkPoint(p.Point, previewBitmap, Color.FromArgb(255, 0, 0), 1));
            fields.Skip((int)(fields.Count * 0.2)).Take((int)(fields.Count * 0.3)).ToList()
                .ForEach(p => MarkPoint(p.Point, previewBitmap, Color.FromArgb(200, 0, 0), 1));
            fields.Skip((int)(fields.Count * 0.5)).ToList()
                .ForEach(p => MarkPoint(p.Point, previewBitmap, Color.FromArgb(155, 0, 0), 1));

            waterAreas.ForEach(w => w.ToList().ForEach(p => MarkPoint(p, previewBitmap, Color.FromArgb(0, 0, 255), 1)));

            waterAreaBoundaryPoints.ForEach(p => { MarkPoint(p, previewBitmap, Color.FromArgb(0, 255, 0)); });

            var fieldGrid = new Field[_heightMap.Width, _heightMap.Height];
            for (int i = 0; i < _heightMap.Width; i++)
            {
                for (int j = 0; j < _heightMap.Height; j++)
                {
                    fieldGrid[i, j] = new Field() { Position = new Point(i, j) };
                }
            }
            foreach (var field in fields)
            {
                fieldGrid[field.Point.X, field.Point.Y].InSettlement = true;
                fieldGrid[field.Point.X, field.Point.Y].DistanceToWater = field.DistanceToWater;
            }

            var rand = new Random();
            var verticalRoad = rand.NextDouble() >= 0.5;
            var min = verticalRoad ? fields.Min(f => f.Point.Y) : fields.Min(f => f.Point.X);
            var max = verticalRoad ? fields.Max(f => f.Point.Y) : fields.Max(f => f.Point.X);
            var startFields = verticalRoad ? fields.Where(f => f.Point.Y == min).ToArray() : fields.Where(f => f.Point.X == min).ToArray();
            var start = startFields[rand.Next(0, startFields.Count())];
            var endFields = verticalRoad ? fields.Where(f => f.Point.Y == max).ToArray() : fields.Where(f => f.Point.X == max).ToArray();
            var end = endFields[rand.Next(0, endFields.Count())];
            var mainRoadPoints = (await FindMainRoad(fieldGrid, start.Point, end.Point)).ToList();
            mainRoadPoints.ForEach(p => { MarkPoint(new Point(p.X, p.Y), previewBitmap, Color.FromArgb(0, 0, 0)); });
            #endregion

            var mStep = mainRoadPoints.Count() / 15;
            var selectedRoadPoints = mainRoadPoints.OrderBy(p => p.X).ThenBy(p => p.Y).Where((x, i) => i % mStep == 0).ToList();
            foreach (var field in fields)
            {
                fieldGrid[field.Point.X, field.Point.Y].DistanceToMainRoad =
                    selectedRoadPoints.Min(p => builderHelper.DistanceTo(field.Point, p));
            }
            selectedRoadPoints.ForEach(p => { MarkPoint(p, previewBitmap, Color.Purple); });
            var settlementInfo = new SettlementInfo()
            {

                PreviewBitmap = previewBitmap,
                MainRoad = mainRoadPoints,
                Fields = fieldGrid

            };
            return settlementInfo;
        }

        private void MarkPoint(Point point, Bitmap bitmap, Color color, int offset = 5)
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

        private IEnumerable<Point> GetPixels(Bitmap bitmap, Func<Color, bool> func, int offset = 10)
        {
            var fastBitmap = new FastBitmap(bitmap);
            fastBitmap.Lock();
            var pixels = new List<Point>();
            for (int y = offset; y < bitmap.Height - offset; y++)
            {
                for (int x = offset; x < bitmap.Width - offset; x++)
                {
                    var pixel = fastBitmap.GetPixel(x, y);
                    if (func(pixel))
                        pixels.Add(new Point(x, y));
                }
            }
            fastBitmap.Unlock();
            return pixels;
        }

        private async Task<IEnumerable<Point>> ApplyFloodFillAsync(
            Bitmap bitmap,
            Point startPoint,
            Func<Color, bool> boundaryFunc)
        {
            var fastBitmap = new FastBitmap(bitmap);
            return await Task.Run(delegate
            {
                Stack<Point> pixels = new Stack<Point>();
                pixels.Push(startPoint);
                var marked = new Dictionary<Point, byte>();
                while (pixels.Count > 0)
                {
                    Point a = pixels.Pop();
                    if (a.X < bitmap.Width && a.X > -1 && a.Y < bitmap.Height && a.Y > -1)
                    {
                        fastBitmap.Lock();
                        var pixel = fastBitmap.GetPixel(a.X, a.Y);
                        if (boundaryFunc(pixel) && !marked.ContainsKey(a))
                        {
                            marked.Add(a, pixel.G);
                            pixels.Push(new Point(a.X - 1, a.Y));
                            pixels.Push(new Point(a.X + 1, a.Y));
                            pixels.Push(new Point(a.X, a.Y - 1));
                            pixels.Push(new Point(a.X, a.Y + 1));
                        }
                        fastBitmap.Unlock();
                    }
                }

                fastBitmap.Lock();
                marked.ToList().ForEach(p =>
                    fastBitmap.SetPixel(p.Key.X, p.Key.Y,
                        Color.FromArgb(p.Value, p.Value, p.Value)));
                fastBitmap.Unlock();

                return marked.Keys;
            });
        }

        private async Task<IEnumerable<Point>> FindMainRoad(Field[,] fieldGrid, Point start, Point end)
        {
            var grid = new Grid(_heightMap.Width, _heightMap.Height, 1.0f);

            for (int i = 0; i < _heightMap.Width; i++)
            {
                for (int j = 0; j < _heightMap.Height; j++)
                {
                    if (!fieldGrid[i, j].InSettlement)
                    {
                        grid.BlockCell(new Position(i, j));
                    }
                }
            }

            var positions = await Task.Run(() => grid.GetPath(new Position(start.X, start.Y), new Position(end.X, end.Y)));

            return positions.Select(p => new Point(p.X, p.Y));
        }
    }
}
