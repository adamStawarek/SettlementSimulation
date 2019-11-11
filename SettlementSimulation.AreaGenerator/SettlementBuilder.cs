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
        private Dictionary<Point, byte> _areaPoints;

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
            _areaPoints = new Dictionary<Point, byte>();
            var potentialAreaPoints = GetPotentialAreaPoints();
            var bitmap = new Bitmap(_heightMap);
            while (potentialAreaPoints.Count > 0 &&
                   potentialAreaPoints.Count > _areaPoints.Count)
            {
                bitmap = await ApplyFloodFillAsync(
                    bitmap,
                    potentialAreaPoints.First(),
                    potentialAreaPoints);
            }

            return (_areaPoints.Select(p => new Field(p.Key)), bitmap);
        }

        private List<Point> GetPotentialAreaPoints()
        {
            int offsetWidth = 10;
            int offsetHeight = 10;
            var potentialArePoints = new List<Point>();
            for (int y = offsetHeight; y < _heightMap.Height - offsetHeight; y++)
            {
                for (int x = offsetWidth; x < _heightMap.Width - offsetWidth; x++)
                {
                    var pixel = _heightMap.GetPixel(x, y);
                    byte intensity = pixel.G;
                    if (intensity >= _minHeight && intensity <= _maxHeight)
                        potentialArePoints.Add(new Point(x, y));
                }
            }

            return potentialArePoints;
        }

        private async Task<Bitmap> ApplyFloodFillAsync(
            Bitmap bitmap,
            Point startPoint,
            List<Point> potentialAreaPoints)
        {
            var fastBitmap = new FastBitmap(bitmap);
            return await Task.Run(delegate
            {
                Stack<Point> pixels = new Stack<Point>();
                fastBitmap.Lock();
                fastBitmap.Unlock();
                pixels.Push(startPoint);
                var marked = new Dictionary<Point, byte>();
                while (pixels.Count > 0)
                {
                    Point a = pixels.Pop();
                    if (a.X < fastBitmap.Width && a.X > -1 && a.Y < fastBitmap.Height && a.Y > -1)
                    {
                        fastBitmap.Lock();
                        var pixelColor = fastBitmap.GetPixel(a.X, a.Y);
                        byte intensity = pixelColor.G;
                        //when pixel intensity is significantly different that target intensity, set this pixel to black
                        if (intensity >= _minHeight && intensity <= _maxHeight && !marked.ContainsKey(a))
                        {
                            fastBitmap.SetPixel(a.X, a.Y, Color.FromArgb(255, 0, 0));
                            marked.Add(a, intensity);
                            pixels.Push(new Point(a.X - 1, a.Y));
                            pixels.Push(new Point(a.X + 1, a.Y));
                            pixels.Push(new Point(a.X, a.Y - 1));
                            pixels.Push(new Point(a.X, a.Y + 1));
                        }
                        fastBitmap.Unlock();
                    }
                }

                potentialAreaPoints.RemoveAll(p => marked.ContainsKey(p));
                //remove pixels that are contained in area from potential area pixels

                fastBitmap.Lock();
                ////when marked size is less than previous picked area we change its color back
                if (marked.Count < _areaPoints.Count)
                    marked.ToList().ForEach(p =>
                        fastBitmap.SetPixel(p.Key.X, p.Key.Y,
                            Color.FromArgb(p.Value, p.Value, p.Value)));
                else
                {
                    _areaPoints.ToList().ForEach(p =>
                        fastBitmap.SetPixel(p.Key.X, p.Key.Y,
                            Color.FromArgb(p.Value, p.Value, p.Value)));
                    _areaPoints.Clear();
                    _areaPoints = marked;
                }
                fastBitmap.Unlock();
                return bitmap;
            });
        }
    }
}
