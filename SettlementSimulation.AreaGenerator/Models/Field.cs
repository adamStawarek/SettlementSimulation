using System.Drawing;

namespace SettlementSimulation.AreaGenerator.Models
{
    public class Field
    {
        public Field(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Field(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public double DistanceToWater { get; set; }
    }
}
