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
        
        public static void GenerateAndPrintMatrices(string path, int amount, int rows, int cols, double sparsity)
        {
            for (var i = 0; i < amount; i++)
            {
                var matrix = GenerateMatrix(rows, cols, sparsity);
                var pathToWrite = Path.Combine(path, "Matrix" + i + ".txt");
                Printer.PrintMatrix(pathToWrite, matrix);
            }
        }
    }
}