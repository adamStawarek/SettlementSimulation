using System.Drawing;

namespace SettlementSimulation.Engine.Models
{
    public class Field
    {
        public Field(int x, int y)
        {
            Location = new Location(x, y);
        }

        public Field(Point point)
        {
            Location = new Location(point.X, point.Y);
        }

        public Location Location { get; }
        public double DistanceToWater { get; set; }
    }
}
