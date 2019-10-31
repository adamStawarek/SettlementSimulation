using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SettlementSimulation.Viewer.ViewModel.Helpers
{
    public class ColorUnderCursor
    {
        [DllImport("gdi32")]
        public static extern uint GetPixel(IntPtr hDC, int XPos, int YPos);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        /// <summary> 
        /// Gets the System.Drawing.Color from under the mouse cursor. 
        /// </summary> 
        /// <returns>The color value.</returns> 
        public static Color Get()
        {
            IntPtr dc = GetWindowDC(IntPtr.Zero);

            POINT p;
            GetCursorPos(out p);

            long color = GetPixel(dc, p.X, p.Y);
            return Color.FromArgb((int)color);
        }
    }
}
