using System.Threading;

namespace Task1
{
    public static class MatrixOperations
    {
        public static Matrix MultiplySequentially(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.Size.columns != matrix2.Size.rows)
                throw new MultiplicationException("The number of columns in the first matrix " +
                    "must be equal to the number of rows in the second.");

            var result = new int[matrix1.Size.rows, matrix2.Size.columns];
            for (var row = 0; row < matrix1.Size.rows; ++row)
            {
                for (var column = 0; column < matrix2.Size.columns; ++column)
                {
                    for (var i = 0; i < matrix1.Size.columns; ++i)
                    {
                        result[row, column] += matrix1[row, i] * matrix2[i, column];
                    }
                }
            }
            return new Matrix(result);
        }

        public static Matrix MultiplyParallel(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.Size.columns != matrix2.Size.rows)
                throw new MultiplicationException("The number of columns in the first matrix " +
                    "must be equal to the number of rows in the second.");

            var result = new int[matrix1.Size.rows, matrix2.Size.columns];
            var threads = new Thread[8];
            var step = threads.Length;

            for (var i = 0; i < threads.Length; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    for (var j = localI; j < result.Length; j += step)
                    {
                        var row = j / result.GetLength(1);
                        var column = j % result.GetLength(1);
                        for (var k = 0; k < matrix1.Size.columns; ++k)
                        {
                            result[row, column] += matrix1[row, k] * matrix2[k, column];
                        }
                    }
                });
            }

            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();

            return new Matrix(result);
        }
    }
}
