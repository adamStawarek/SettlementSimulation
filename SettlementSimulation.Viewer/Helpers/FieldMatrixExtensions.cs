using SettlementSimulation.AreaGenerator.Models;
using System.Collections.Generic;

namespace SettlementSimulation.Viewer.Helpers
{
    public static class FieldMatrixExtensions
    {
        public static List<Field> ToList(this Field[,] fieldMatrix)
        {
            var fields = new List<Field>();
            for (int i = 0; i < fieldMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < fieldMatrix.GetLength(1); j++)
                {
                    fields.Add(fieldMatrix[i, j]);
                }
            }

            return fields;
        }
    }
}