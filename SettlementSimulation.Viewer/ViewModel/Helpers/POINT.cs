using System.Runtime.InteropServices;

namespace SettlementSimulation.Viewer.ViewModel.Helpers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            var other = (POINT)obj;
            return other.X == this.X && other.Y == this.Y;
        }
    }
}