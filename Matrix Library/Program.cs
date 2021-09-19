using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MatrixLibrary
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Wrong number of arguments.");
                return;
            }
            
            long[,] firstMatrix;
            long[,] secondMatrix;
            try
            {
                firstMatrix = RiderPrinter.Read(args[0]);
                secondMatrix = RiderPrinter.Read(args[1]);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Empty file.");
                return;
            }
            catch (FormatException)
            {
                Console.WriteLine("Wrong format.");
                return;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Invalid path.");
                return;
            }

            long[,] result;
            try
            {
                result = Multiplication.MultiThreaded(firstMatrix, secondMatrix);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid matrices sizes.");
                return;
            }

            try
            {
                RiderPrinter.Print(args[2], result);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Invalid path.");
                return;
            }
            
            
            static void TestFunc(int iterations, int size) // Method for performance test.
            {
                var resultsSingleThreaded = new List<long>();
                var resultsMultiThreaded = new List<long>();
                var timer = new Stopwatch();
                for (var i = 0; i < iterations; i++)
                {
                    var rand = new Random();
                    var matrixA = Generator.GenerateMatrix(size, size, rand.NextDouble());
                    var matrixB = Generator.GenerateMatrix(size, size, rand.NextDouble());

                    timer.Restart();
                    Multiplication.SingleThreaded(matrixA, matrixB);
                    timer.Stop();
                    resultsSingleThreaded.Add(timer.ElapsedMilliseconds);

                    timer.Restart();
                    Multiplication.MultiThreaded(matrixA, matrixB);
                    timer.Stop();
                    resultsMultiThreaded.Add(timer.ElapsedMilliseconds);
                }
                
                static (double, double) FindAverageAndStandardDeviation(IReadOnlyCollection<long> results)
                {
                    var average = results.Average();
                    var variance = results.Select(x => 
                        Math.Pow(x - average, 2)).Average();
                    var standardDeviation = Math.Sqrt(variance);
                    return (average, standardDeviation);
                }

                var (averageSingleThreaded, standardDeviationSingleThreaded) = 
                    FindAverageAndStandardDeviation(resultsSingleThreaded);
                var (averageMultiThreaded, standardDeviationMultiThreaded) =
                    FindAverageAndStandardDeviation(resultsMultiThreaded);
                Console.WriteLine("Average time for single-threaded: " + averageSingleThreaded + " ms");
                Console.WriteLine("Standard deviation for single-threaded: " + standardDeviationSingleThreaded + " ms");
                Console.WriteLine("Average time for multi-threaded: " + averageMultiThreaded + " ms");
                Console.WriteLine("Standard deviation for multi-threaded: " + standardDeviationMultiThreaded + " ms");
            }

            TestFunc(50, 128);
            TestFunc(50, 256);
            TestFunc(50, 512);
            TestFunc(50, 1024);
            
            /*
            Была создана тестовая функция, которая заданное количество раз создаёт пары случайных матриц и перемножает их.
            В итоге вычисляется среднее время выполнения среди всех итераций.
            Были проведены тесты для квадратных матриц размерами 128, 256, 512, 1024. Количество итераций - 10.
            Результаты: 
            128x128:    single-threaded: 17.1 ms
                        multi-threaded: 8.4 ms
            256x256:    single-threaded: 154.2 ms
                        multi-threaded: 49.2 ms
            512x512:    single-threaded: 1286.3 ms
                        multi-threaded: 200.4 ms
            1024x1024:  single-threaded: 38358.8 ms
                        multi-threaded: 4300.8 ms                                                                   
            */
        }
    }
}