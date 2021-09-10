using System;

namespace Matrix_Library
{
    public static class Multiplication
    {
        public static long[,] SingleThreaded(long[,] matrixA, long[,] matrixB)
        {
            var rows1 = matrixA.GetLength(0);
            var cols2 = matrixB.GetLength(1);
            var result = new long[rows1, cols2];
            if (rows1 != cols2) throw new Exception("Invalid matrices sizes");
            for (var i = 0; i < rows1; i++)
            {
                for (var k = 0; k < cols2; k++)
                {
                    for (var r = 0; r < rows1; r++)
                    {
                        result[i, k] += matrixA[i, r] * matrixB[r, k];
                    }
                }
            }
            return result;
        }
    }
}