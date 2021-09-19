using System;

namespace MatrixLibrary
{
    /// <summary>
    /// Implementing matrix generator
    /// </summary>
    public static class Generator
    {
        /// <summary>
        /// Generates random matrix.
        /// </summary>
        /// <param name="rows">Numbers of rows in generated matrix.</param>
        /// <param name="cols">Numbers of cols in generated matrix.</param>
        /// <param name="sparsity">Sparsity of matrix.</param>
        /// <returns>long[,]</returns>
        /// <exception cref="ArgumentOutOfRangeException">rows below 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException">cols below 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException">sparsity below 0.0 or above 1.0.</exception>
        public static long[,] GenerateMatrix(int rows, int cols, double sparsity)
        {
            if (rows <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rows),"Invalid number of rows");
            }
            if (cols <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cols),"Invalid number of cols");
            }
            if (sparsity is < 0.0 or > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(sparsity),"Invalid sparsity");
            }

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