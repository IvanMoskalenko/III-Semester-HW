﻿using System;
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
            В итоге вычисляется среднее время выполнения среди всех итераций и среднеквадратичное отклонение.
            Были проведены тесты для квадратных матриц размерами 128, 256, 512, 1024. Количество итераций - 50.
            Результаты: 
            128x128:    single-threaded: 17.56 ms, standard deviation: 3.47655001402252 ms
                        multi-threaded: 8.88 ms, standard deviation: 1.2905812643921342 ms
            256x256:    single-threaded: 149.7 ms, standard deviation: 13.35702062587312 ms
                        multi-threaded: 50.92 ms, standard deviation: 4.894241514269602 ms
            512x512:    single-threaded: 1263.08 ms, standard deviation: 60.091210671777944 ms
                        multi-threaded: 207.6 ms, standard deviation: 5.528109984434101 ms
            1024x1024:  single-threaded: 26303.52 ms, standard deviation: 659.4704008520777 ms
                        multi-threaded: 3138.02 ms, standard deviation: 313.6486244191101 ms                                                                   
            */
        }
    }
}