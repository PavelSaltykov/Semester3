using NUnit.Framework;
using System;

namespace Task1.Tests
{
    [TestFixture()]
    public class MatrixOperationsTests
    {
        private static readonly Func<Matrix, Matrix, Matrix>[] multiplications =
        {
            (matrix1, matrix2) => MatrixOperations.MultiplySequentially(matrix1, matrix2),
            (matrix1, matrix2) => MatrixOperations.MultiplyParallel(matrix1, matrix2)
        };

        [TestCaseSource(nameof(multiplications))]
        public void MultiplicationExceptionTest(Func<Matrix, Matrix, Matrix> multiply)
        {
            var matrix1 = new Matrix(new int[1, 1] { { 1 } });
            var matrix2 = new Matrix(new int[2, 1] { { 1 }, { 2 } });
            Assert.Throws<MultiplicationException>(() => multiply(matrix1, matrix2));
        }

        [TestCaseSource(nameof(multiplications))]
        public void MultiplyTest(Func<Matrix, Matrix, Matrix> multiply)
        {
            var array1 = new int[2, 3] { { 1, 2, 3 },
                                         { 4, 5, 6 } };

            var array2 = new int[3, 2] { { -2, -1 },
                                         { 0, 1 },
                                         { 2, 3 } };

            var expectedArray = new int[2, 2] { { 4, 10 },
                                                { 4, 19 } };

            var matrix1 = new Matrix(array1);
            var matrix2 = new Matrix(array2);
            var expected = new Matrix(expectedArray);

            Assert.AreEqual(expected, multiply(matrix1, matrix2));
        }

        private void FillArrays(int[,] array1, int[,] array2, int[,] expectedArray)
        {
            var rnd = new Random();
            for (var i = 0; i < array1.GetLength(0); ++i)
            {
                for (var j = 0; j < array1.GetLength(1); ++j)
                {
                    array1[i, j] = rnd.Next(-10, 10);
                }
            }

            for (var i = 0; i < array2.GetLength(0); ++i)
            {
                for (var j = 0; j < array2.GetLength(1); ++j)
                {
                    array2[i, j] = rnd.Next(-10, 10);
                }
            }

            for (var i = 0; i < expectedArray.Length; ++i)
            {
                var row = i / expectedArray.GetLength(1);
                var column = i % expectedArray.GetLength(1);
                for (var j = 0; j < array1.GetLength(1); ++j)
                {
                    expectedArray[row, column] += array1[row, j] * array2[j, column];
                }
            }
        }

        [TestCaseSource(nameof(multiplications))]
        public void BigMatricesTest(Func<Matrix, Matrix, Matrix> multiply)
        {
            var array1 = new int[100, 150];
            var array2 = new int[150, 200];
            var expectedArray = new int[100, 200];

            FillArrays(array1, array2, expectedArray);

            var matrix1 = new Matrix(array1);
            var matrix2 = new Matrix(array2);
            var expected = new Matrix(expectedArray);

            Assert.AreEqual(expected, multiply(matrix1, matrix2));
        }
    }
}