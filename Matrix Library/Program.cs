namespace Matrix_Library
{
    static class Program
    {
        static void Main(string[] args)
        {
            //Generator.GenerateAndPrintMatrices("/home/ivan/Documents/test2", 100, 10, 10, 0.9);
            var matrixA = Generator.GenerateMatrix(3, 3, 0.5);
            var matrixB = Generator.GenerateMatrix(3, 3, 0.5);
            var result = Multiplication.SingleThreaded(matrixA, matrixB);
            Printer.PrintMatrix("/home/ivan/Documents/test2/matrixA.txt", matrixA);
            Printer.PrintMatrix("/home/ivan/Documents/test2/matrixB.txt", matrixB);
            Printer.PrintMatrix("/home/ivan/Documents/test2/result.txt", result);
        }
    }
}