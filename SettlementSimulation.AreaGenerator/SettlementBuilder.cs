using System;
using FastBitmapLib;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SettlementSimulation.AreaGenerator.Models;

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

        public async Task<(IEnumerable<Field>, Bitmap)> BuildAsync()
        {
            var colorMap = new Bitmap(_colorMap);
            var waterAreas = new List<IEnumerable<Point>>();
            var potentialWaterPointsWithPixels = 
                GetPixels(colorMap, color => color.B>=byte.MaxValue-5&&color.R<=byte.MinValue+5
                                                                      &&color.G>=50 && color.G<=120)
                    .OrderBy(p => GetIntensity(p.pixel))
                    .ToList();

            var potentialWaterPoints = potentialWaterPointsWithPixels
                .Select(p => p.point)
                .ToList();

          
            while (potentialWaterPoints.Count > 0)
            {
                var area = await ApplyFloodFillAsync(
                    colorMap,
                    potentialWaterPoints.First(),
                    color => color.B >= byte.MaxValue - 5 && color.R <= byte.MinValue + 5
                                                                   && color.G >= 50 && color.G <= 120);

                potentialWaterPoints.RemoveAll(p => area.Contains(p));

                waterAreas.Add(area);
            }

            var maxWaterArea = waterAreas.Max(w => w.Count());
            waterAreas.RemoveAll(w => w.Count() < 0.4 * maxWaterArea);

            var bitmap = new Bitmap(_heightMap);
            var areas = new List<IEnumerable<Point>>();
            var potentialAreaPoints = GetPixels(_heightMap, color => color.G >= _minHeight && color.G <= _maxHeight)
                .Select(p=>p.point)
                .ToList();

            while ( potentialAreaPoints.Count > 0 )
            {
                var area = await ApplyFloodFillAsync(
                    bitmap,
                    potentialAreaPoints.First(),
                    color => color.G >=_minHeight && color.G<=_maxHeight);

                potentialAreaPoints.RemoveAll(p => area.Contains(p));

                areas.Add(area);
            }

            var selectedArea = areas.OrderByDescending(a => a.Count()).First().ToList();//TODO
           
            selectedArea.ForEach(p =>
                bitmap.SetPixel(p.X, p.Y,
                    Color.Red));

            waterAreas.ForEach(w=>w.ToList().ForEach(p => bitmap.SetPixel(p.X, p.Y,
                Color.Blue)));

            return (selectedArea.Select(p => new Field(p)), bitmap);
        }

        private IEnumerable<(Point point, Color pixel)> GetPixels(Bitmap bitmap, Func<Color, bool> func, int offset=10)
        {
            var fastBitmap=new FastBitmap(bitmap);
            fastBitmap.Lock();
            var pixels = new List<(Point,Color)>();
            for (int y = offset; y < bitmap.Height - offset; y++)
            {
                for (int x = offset; x < bitmap.Width - offset; x++)
                {
                    var pixel = fastBitmap.GetPixel(x, y);
                    if (func(pixel))
                        pixels.Add((new Point(x, y),pixel));
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
                        //when pixel intensity is different that target intensity, set this pixel to black
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

        private int GetIntensity(Color color)
        {
            return color.R + color.G + color.B;
        }
    }
}
