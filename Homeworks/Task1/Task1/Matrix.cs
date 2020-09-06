using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task1
{
    /// <summary>
    /// Represents a rectangular table of integer elements.
    /// </summary>
    public class Matrix
    {
        private readonly int[,] elements;

        /// <summary>
        /// Returns the number of rows and columns.
        /// </summary>
        public (int rows, int columns) Size => (elements.GetLength(0), elements.GetLength(1));

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="elements">Two-dimensional array of integer elements.</param>
        public Matrix(int[,] elements)
        {
            if (elements.GetLength(0) == 0 || elements.GetLength(1) == 0)
                throw new ArgumentException();
            this.elements = (int[,])elements.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="filename">Name of the file containing a matrix.</param>
        /// <exception cref="InvalidMatrixFileException">Thrown when the file contains an invalid matrix.</exception>
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
            if (rows == 0)
                throw new InvalidMatrixFileException("File is empty");

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

        /// <summary>
        /// Gets or sets the element at the specified position.
        /// </summary>
        /// <param name="row">The zero-based row number.</param>
        /// <param name="column">The zero-based column number.</param>
        /// <returns>The element at the specified position.</returns>
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

        /// <summary>
        /// Writes the matrix to the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to write.</param>
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
