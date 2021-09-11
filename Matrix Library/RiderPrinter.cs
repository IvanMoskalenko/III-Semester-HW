using System;
using System.IO;

namespace Matrix_Library
{
    public static class RiderPrinter
    {
        public static void PrintMatrix(string path, long[,] array)
        {
            var x = array.GetLength(0);
            var y = array.GetLength(1);
            var text = "";
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    text = text + array[i, j] + " ";
                }
                text = text.TrimEnd() + "\n";
            }
            File.WriteAllText(path, text);
        }
        
        public static long[,] ReadMatrix(string path)
        {
            var fileLines = File.ReadAllLines(path);
            var matrix = new long[fileLines.Length, fileLines[0].Split(' ').Length];
            for (var i = 0; i < fileLines.Length; i++)
            {
                var line = fileLines[i];
                for (var j = 0; j < matrix.GetLength(1); ++j)
                {
                    var split = line.Split(' ');
                    matrix[i, j] = Convert.ToInt64(split[j]);
                }
            }
            return matrix;
        }
    }
}