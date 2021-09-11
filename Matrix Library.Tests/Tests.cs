using System;
using NUnit.Framework;

namespace Matrix_Library.Tests
{
    public class Tests
    {
        [Test]
        public void MultiplicationTest()
        {
            var matrix1 = new long[,] { { 1, 2 }, { 3, 4 } };
            var matrix2 = new long[,] { { 5, 2 }, { 5, 4 } };
            var rightResult1 = new long[,] { { 15, 10 }, { 35, 22 } };
            var resultMultiThreaded1 = Multiplication.MultiThreaded(matrix1, matrix2);
            var resultSingleThreaded1 = Multiplication.SingleThreaded(matrix1, matrix2);
            Assert.AreEqual(rightResult1, resultMultiThreaded1);
            Assert.AreEqual(rightResult1, resultSingleThreaded1);
            
            var matrix3 = new long[,] { { 1, 2 }, { 3, 4 }, {6, 9} };
            var matrix4 = new long[,] { { 5, 2, 7 }, { 5, 4, 9 } };
            var rightResult2 = new long[,] { { 15, 10, 25 }, { 35, 22, 57 }, { 75, 48, 123 } };
            var resultMultiThreaded2 = Multiplication.MultiThreaded(matrix3, matrix4);
            var resultSingleThreaded2 = Multiplication.SingleThreaded(matrix3, matrix4);
            Assert.AreEqual(rightResult2, resultMultiThreaded2);
            Assert.AreEqual(rightResult2, resultSingleThreaded2);
            
            var matrix5 = new long[,] { { 5 } };
            var matrix6 = new long[,] { { 7 } };
            var rightResult3 = new long[,] { { 35 } };
            var resultMultiThreaded3 = Multiplication.MultiThreaded(matrix5, matrix6);
            var resultSingleThreaded3 = Multiplication.SingleThreaded(matrix5, matrix6);
            Assert.AreEqual(rightResult3, resultMultiThreaded3);
            Assert.AreEqual(rightResult3, resultSingleThreaded3);
        }

        [Test]
        public void RandomizedMultiplicationTest()
        {
            for (var i = 0; i < 100; i++)
            {
                var rand = new Random();
                var rows1 = rand.Next(1, 20);
                var cols1AndRows1 = rand.Next(1, 20);
                var cols2 = rand.Next(1, 20);
                var matrixA = Generator.GenerateMatrix(rows1, cols1AndRows1, 0.5);
                var matrixB = Generator.GenerateMatrix(cols1AndRows1, cols2, 0.5);
                var resultSingleThreaded = Multiplication.SingleThreaded(matrixA, matrixB);
                var resultMultiThreaded = Multiplication.MultiThreaded(matrixA, matrixB);
                Assert.AreEqual(resultSingleThreaded, resultMultiThreaded);
            }
        }
        
        [Test]
        public void MultiplicationExceptionTest()
        {
            var matrixA = new long[1, 5];
            var matrixB = new long[1, 5];
            Assert.Throws<Exception> (() => Multiplication.SingleThreaded(matrixA, matrixB));
            Assert.Throws<Exception> (() => Multiplication.MultiThreaded(matrixA, matrixB));
        }
    }
}