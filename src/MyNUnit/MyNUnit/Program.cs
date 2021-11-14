using System;
using System.IO;

namespace MyNUnit
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Wrong number of arguments. Please, specify only path to directory");
            }
            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("Specified directory doesn't exist");
                return;
            }
            try
            {
                var tests = new MyNUnit(args[0]);
                long totalTime = 0;
                foreach (var test in tests.TestsInformation)
                {
                    totalTime += test.Time;
                    Console.WriteLine($"{test.Name} {test.Result} {test.Time}ms {test.IgnoreReason}");
                }
                Console.WriteLine($"Total time: {totalTime}ms");
            }
            catch (AggregateException)
            {
                Console.WriteLine($"Error was occured in BeforeClass or AfterClass");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}