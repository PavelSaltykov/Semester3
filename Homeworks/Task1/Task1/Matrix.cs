using System.Collections.Generic;
using System.IO;

namespace Task1
{
    public class Matrix
    {
        public int[,] Elements { get; private set; }
        public (int rows, int columns) Size => (Elements.GetLength(0), Elements.GetLength(1));

        public Matrix(int[,] elements) => Elements = elements;

        public Matrix(string filename) : this(MatrixFromFile(filename))
        {
        }

        private static int[,] MatrixFromFile(string filename)
        {
            string firstLine;
            var matrix = new List<string>();
            using (var streamReader = new StreamReader(filename))
            {
                firstLine = streamReader.ReadLine();
                matrix.Add(streamReader.ReadToEnd());
            }

            if (firstLine == null)
                throw new InvalidMatrixFileException("Matrix size is not specified");

            var numbers = firstLine.Split(' ');
            if (numbers.Length != 2)
                throw new InvalidMatrixFileException("Invalid matrix size");

            var size = new int[2];
            for (var i = 0; i < 2; ++i)
            {
                if (!int.TryParse(numbers[i], out size[i]))
                    throw new InvalidMatrixFileException("Invalid matrix size");
            }

            return ConvertToArray(matrix, size[0], size[1]);
        }

        private static int[,] ConvertToArray(List<string> matrix, int rows, int columns)
        {
            var array = new int[rows, columns];
            if (matrix.Count != rows)
                throw new InvalidMatrixFileException("Matrix size is not match");

            for (var i = 0; i < rows; ++i)
            {
                var numbers = matrix[i].Split(' ');
                if (numbers.Length != columns)
                    throw new InvalidMatrixFileException("Matrix size is not match");

                for (var j = 0; j < columns; ++j)
                {
                    if (!int.TryParse(numbers[i], out array[i, j]))
                        throw new InvalidMatrixFileException("Invalid matrix element");
                }
            }
            return array;
        }

        public void WriteToFile(string filename)
        {
            using var streamWriter = new StreamWriter(filename, false);
            streamWriter.WriteLine($"{Size.rows} {Size.columns}");

            for (var i = 0; i < Size.rows; ++i)
            {
                for (var j = 0; j < Size.columns; ++j)
                {
                    streamWriter.Write($"{Elements[i, j]} ");
                }
                streamWriter.WriteLine();
            }
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
                    if (Elements[i, j] != otherMatrix.Elements[i, j])
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode() => Elements.GetHashCode();
    }
}
