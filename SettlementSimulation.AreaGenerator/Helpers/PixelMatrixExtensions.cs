using System.Collections.Generic;
using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.AreaGenerator.Helpers
{
    public static class PixelMatrixExtensions
    {
        public static byte[] ToByteArray(this Pixel[,] matrix)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Pixel[] temp = new Pixel[matrix.GetLength(1)];
                for (int n = 0; n < temp.Length; n++)
                {
                   bytes.Add(matrix[i, n].Intensity);
                }
            }

            return bytes.ToArray();
        }
    }
}
