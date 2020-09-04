namespace Task1
{
    public static class MatrixOperations
    {
        public static Matrix MultiplySequentially(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.Size.columns != matrix2.Size.rows)
                throw new MultiplicationException();

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
    }
}
