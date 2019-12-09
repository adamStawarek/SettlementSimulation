using System;
using System.Collections.Generic;
using System.Text;
using SettlementSimulation.AreaGenerator.Models;

namespace SettlementSimulation.Engine.Helpers
{
    public static class FieldMatrixExtensions
    {
        public static List<Field> ToList(this Field[,] fieldMatrix)
        {
            List<Field> fields = new List<Field>();
            for (int i = 0; i < fieldMatrix.GetLength(0); i++)
            {
                Field[] temp = new Field[fieldMatrix.GetLength(1)];
                for (int n = 0; n < temp.Length; n++)
                {
                    fields.Add(fieldMatrix[i, n]);
                }
            }

            return fields;
        }
    }
}
