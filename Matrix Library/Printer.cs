using System.IO;

namespace Matrix_Library
{
    public static class Printer
    {
        public static void PrintMatrix(string path, int[,] array)
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
                text += "\n";
            }
            File.WriteAllText(path, text);
        }
    }
}