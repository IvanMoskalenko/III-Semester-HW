using System;
using System.Threading;

namespace Matrix_Library
{
    public static class Multiplication
    {
        public static long[,] SingleThreaded(long[,] matrixA, long[,] matrixB)
        {
            var rows1 = matrixA.GetLength(0);
            var cols1 = matrixA.GetLength(1);
            var rows2 = matrixB.GetLength(0);
            var cols2 = matrixB.GetLength(1);
            if (cols1 != rows2) throw new Exception("Invalid matrices sizes");
            var result = new long[rows1, cols2];
            for (var i = 0; i < rows1; i++)
            {
                for (var k = 0; k < cols2; k++)
                {
                    for (var r = 0; r < cols1; r++)
                    {
                        result[i, k] += matrixA[i, r] * matrixB[r, k];
                    }
                }
            }
            return result;
        }
        
        public static long[,] MultiThreaded(long[,] matrixA, long[,] matrixB)
        {
            var rows1 = matrixA.GetLength(0);
            var cols1 = matrixA.GetLength(1);
            var rows2 = matrixB.GetLength(0);
            var cols2 = matrixB.GetLength(1);
            if (cols1 != rows2) throw new Exception("Invalid matrices sizes");
            var threads = new Thread[Math.Min(Environment.ProcessorCount, rows1)];
            var chunkSize = rows1 / threads.Length + 1;
            var result = new long[rows1, cols2];
            for (var i = 0; i < threads.Length; i++)
            {
                var local = i;
                threads[i] = new Thread(() =>
                {
                    for (var z= local * chunkSize; z < Math.Min((local + 1) * chunkSize, rows1); z++)
                    {
                        for (var k = 0; k < cols2; k++)
                        {
                            for (var r = 0; r < cols1; r++)
                            {
                                result[z, k] += matrixA[z, r] * matrixB[r, k];
                            }
                        }
                    }
                });
            }
            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();
            return result;
        }
    }
}