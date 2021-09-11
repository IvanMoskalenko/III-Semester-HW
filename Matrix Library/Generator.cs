using System;
using System.IO;

namespace Matrix_Library
{
    public static class Generator
    {
        public static long[,] GenerateMatrix(int rows, int cols, double sparsity)
        {
            var rand = new Random();
            var output = new long[rows, cols];
            for (var j = 0; j < rows; j++)
            {
                for (var k = 0; k < cols; k++)
                {
                    var y = rand.NextDouble();
                    if (y > sparsity)
                    {
                        output[j, k] = rand.Next();
                    }
                    else
                    {
                        output[j, k] = 0;
                    }
                }
            }
            return output;
        }
    }
}