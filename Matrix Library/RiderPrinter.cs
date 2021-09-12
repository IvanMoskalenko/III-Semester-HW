using System;
using System.IO;

namespace Matrix_Library
{
    public static class RiderPrinter
    {
        /// <summary>
        /// Prints matrix to specified file.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <param name="array">Matrix to print.</param>
        /// <exception cref="FileNotFoundException">invalid path.</exception>
        public static void Print(string path, long[,] array)
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

        /// <summary>
        /// Reads matrix from specified file.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <returns>long[,] - read matrix.</returns>
        /// <exception cref="IndexOutOfRangeException">empty file.</exception>
        /// <exception cref="FormatException">invalid matrix format.</exception>
        /// <exception cref="FileNotFoundException">invalid path.</exception>
        public static long[,] Read(string path)
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