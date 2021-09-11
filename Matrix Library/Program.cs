using System;
using System.Diagnostics;

namespace Matrix_Library
{
    static class Program
    {
        static void Main(string[] args)
        {
            //Generator.GenerateAndPrintMatrices("/home/ivan/Documents/test2", 100, 10, 10, 0.9);
            var matrixA = Generator.GenerateMatrix(1000, 1000, 0.8);
            var matrixB = Generator.GenerateMatrix(1000, 1000, 0.8);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = Multiplication.SingleThreaded(matrixA, matrixB);
            stopwatch.Stop();
            Console.WriteLine("ms: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
            var result2 = Multiplication.MultiThreaded(matrixA, matrixB);
            stopwatch.Stop();
            Console.WriteLine("ms: " + stopwatch.ElapsedMilliseconds);
            // Printer.PrintMatrix("/home/ivan/Documents/test2/matrixA.txt", matrixA);
            // Printer.PrintMatrix("/home/ivan/Documents/test2/matrixB.txt", matrixB);
            // Printer.PrintMatrix("/home/ivan/Documents/test2/result.txt", result);
            // Printer.PrintMatrix("/home/ivan/Documents/test2/result2.txt", result2);
        }
    }
}