using System;
using System.Diagnostics;
using System.IO;

namespace Task1
{
    public class Program
    {
        private static void GenerateMatrixInFile(string filename, (int rows, int columns) size)
        {
            var rnd = new Random();
            using var streamWriter = new StreamWriter(filename, false);
            for (var i = 0; i < size.rows; ++i)
            {
                for (var j = 0; j < size.columns; ++j)
                {
                    streamWriter.Write($"{rnd.Next(-1000, 1000)} ");
                }
                streamWriter.WriteLine();
            }
        }

        public static void Main(string[] args)
        {
            var filename1 = "Matrix1.txt";
            var filename2 = "Matrix2.txt";
            var resultFilename = "Result.txt";
            var size1 = (1000, 500);
            var size2 = (500, 800);

            GenerateMatrixInFile(filename1, size1);
            GenerateMatrixInFile(filename2, size2);

            Console.WriteLine($"Size of the first matrix: {size1}");
            Console.WriteLine($"Size of the second matrix: {size2}");
            Console.WriteLine();

            var matrix1 = new Matrix(filename1);
            var matrix2 = new Matrix(filename2);

            try
            {
                Console.Write("Time of the sequential multiplication: ");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                MatrixOperations.MultiplySequentially(matrix1, matrix2);
                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed);

                Console.Write("Time of the parallel multiplication: ");
                stopwatch = Stopwatch.StartNew();
                var resultMatrix = MatrixOperations.MultiplyParallel(matrix1, matrix2);
                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed);

                resultMatrix.WriteToFile(resultFilename);
            }
            catch (MultiplicationException e)
            {
                Console.WriteLine();
                Console.WriteLine(e.Message);
            }
        }
    }
}
