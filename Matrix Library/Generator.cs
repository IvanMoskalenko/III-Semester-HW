using System;

namespace Matrix_Library
{
    public static class Generator
    {
        /// <summary>
        /// Generates random matrix.
        /// </summary>
        /// <param name="rows">Numbers of rows in generated matrix.</param>
        /// <param name="cols">Numbers of cols in generated matrix.</param>
        /// <param name="sparsity">Sparsity of matrix.</param>
        /// <returns>long[,]</returns>
        /// <exception cref="Exception">rows or cols below 1.</exception>
        /// <exception cref="Exception">sparsity below 0.0 or above 1.0.</exception>
        public static long[,] GenerateMatrix(int rows, int cols, double sparsity)
        {
            if (rows <= 0 | cols <= 0) throw new Exception("Invalid matrix sizes");
            if (sparsity < 0.0 | sparsity > 1.0) throw new Exception("Invalid sparsity");
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