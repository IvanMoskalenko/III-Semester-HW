using System;
using System.Threading;

namespace MatrixLibrary
{
    /// <summary>
    /// Implementing matrix multiplication
    /// </summary>
    public static class Multiplication
    {
        /// <summary>
        /// Single-threaded realisation of matrix multiplication.
        /// </summary>
        /// <param name="matrixA">First matrix to multiply.</param>
        /// <param name="matrixB">Second matrix to multiply.</param>
        /// <returns>long[,] - multiplication product.</returns>
        /// <exception cref="ArgumentException">cols of first matrix not equal to rows of second matrix.</exception>
        public static long[,] SingleThreaded(long[,] matrixA, long[,] matrixB)
        {
            var rows1 = matrixA.GetLength(0);
            var cols1 = matrixA.GetLength(1);
            var rows2 = matrixB.GetLength(0);
            var cols2 = matrixB.GetLength(1);
            if (cols1 != rows2)
            {
                throw new ArgumentException("Invalid matrices sizes");
            }

            var result = new long[rows1, cols2];
            for (var row = 0; row < rows1; row++)
            {
                for (var col = 0; col < cols2; col++)
                {
                    for (var i = 0; i < cols1; i++)
                    {
                        result[row, col] += matrixA[row, i] * matrixB[i, col];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Multi-threaded realisation of matrix multiplication.
        /// </summary>
        /// <param name="matrixA">First matrix to multiply.</param>
        /// <param name="matrixB">Second matrix to multiply.</param>
        /// <returns>long[,] - multiplication product.</returns>
        /// <exception cref="ArgumentException">cols of first matrix not equal to rows of second matrix.</exception>
        public static long[,] MultiThreaded(long[,] matrixA, long[,] matrixB)
        {
            var rows1 = matrixA.GetLength(0);
            var cols1 = matrixA.GetLength(1);
            var rows2 = matrixB.GetLength(0);
            var cols2 = matrixB.GetLength(1);
            if (cols1 != rows2)
            {
                throw new ArgumentException("Invalid matrices sizes");
            }

            var threads = new Thread[Math.Min(Environment.ProcessorCount, rows1)];
            var chunkSize = rows1 / threads.Length + 1;
            var result = new long[rows1, cols2];
            for (var i = 0; i < threads.Length; i++)
            {
                var local = i;
                threads[i] = new Thread(() =>
                {
                    for (var row = local * chunkSize; row < Math.Min((local + 1) * chunkSize, rows1); row++)
                    {
                        for (var col = 0; col < cols2; col++)
                        {
                            for (var j = 0; j < cols1; j++)
                            {
                                result[row, col] += matrixA[row, j] * matrixB[j, col];
                            }
                        }
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return result;
        }
    }
}