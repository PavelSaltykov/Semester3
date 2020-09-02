using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task1
{
    public class Matrix
    {
        private readonly int[,] elements;
        public (int rows, int columns) Size => (elements.GetLength(0), elements.GetLength(1));

        public Matrix(int[,] elements) => this.elements = (int[,])elements.Clone();

        public Matrix(string filename) : this(MatrixFromFile(filename))
        {
        }

        private static int[,] MatrixFromFile(string filename)
        {
            var matrix = new List<string>();
            using (var streamReader = new StreamReader(filename))
            {
                while (!streamReader.EndOfStream)
                {
                    matrix.Add(streamReader.ReadLine());
                }
            }
            return ConvertToArray(matrix);
        }

        private static int[,] ConvertToArray(List<string> matrix)
        {
            var rows = matrix.Count();
            var columns = matrix[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Count();
            var array = new int[rows, columns];

            for (var i = 0; i < rows; ++i)
            {
                var numbers = matrix[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (numbers.Length != columns)
                    throw new InvalidMatrixFileException("Invalid matrix");

                for (var j = 0; j < columns; ++j)
                {
                    if (!int.TryParse(numbers[j], out array[i, j]))
                        throw new InvalidMatrixFileException("Invalid matrix element");
                }
            }
            return array;
        }

        public int this[int row, int column]
        {
            get => elements[row, column];
            set => elements[row, column] = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var otherMatrix = (Matrix)obj;
            if (otherMatrix.Size != Size)
                return false;

            for (var i = 0; i < Size.rows; ++i)
            {
                for (var j = 0; j < Size.columns; ++j)
                {
                    if (elements[i, j] != otherMatrix.elements[i, j])
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode() => elements.GetHashCode();

        public void WriteToFile(string filename)
        {
            using var streamWriter = new StreamWriter(filename, false);
            for (var i = 0; i < Size.rows; ++i)
            {
                for (var j = 0; j < Size.columns; ++j)
                {
                    streamWriter.Write($"{elements[i, j]} ");
                }
                streamWriter.WriteLine();
            }
        }
    }
}
