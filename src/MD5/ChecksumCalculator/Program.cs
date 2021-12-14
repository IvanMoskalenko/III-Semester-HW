global using System;
global using System.Collections.Generic;
global using System.Linq;
using System.Diagnostics;

if (args.Length != 1)
{
    Console.WriteLine("Incorrect number of arguments. Please, specify only path to directory or file");
    return;
}

try
{
    var hash = ChecksumCalculator.ChecksumCalculator.Calculate(args[0], true);
    Console.WriteLine(BitConverter.ToString(hash));
}
catch (ArgumentException error)
{
    Console.WriteLine(error.Message);
}

var resultsSingleThreaded = new List<long>();
var resultsMultiThreaded = new List<long>();
var timer = new Stopwatch();
for (var i = 0; i < 10; i++)
{
    timer.Restart();
    ChecksumCalculator.ChecksumCalculator.Calculate(args[0], false);
    timer.Stop();
    resultsSingleThreaded.Add(timer.ElapsedMilliseconds);

    timer.Restart();
    ChecksumCalculator.ChecksumCalculator.Calculate(args[0], true);
    timer.Stop();
    resultsMultiThreaded.Add(timer.ElapsedMilliseconds);
}

Console.WriteLine($"Single threaded average: {resultsSingleThreaded.Average()}");
Console.WriteLine($"Multi threaded average: {resultsMultiThreaded.Average()}");

// Single threaded average: 3496.3
// Multi threaded average: 767.5