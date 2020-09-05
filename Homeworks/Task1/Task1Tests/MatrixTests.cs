using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Task1.Tests
{
    [TestFixture()]
    public class MatrixTests
    {
        [Test]
        public void MatrixFromArrayTest()
        {
            var array = new int[1, 1] { { 0 } };
            var matrix = new Matrix(array);

            array[0, 0] = -1;
            Assert.AreNotEqual(array[0, 0], matrix[0, 0]);
        }

        [Test]
        public void EqualsTest()
        {
            var array1 = new int[1, 1] { { 1 } };
            var matrix1 = new Matrix(array1);

            var array2 = new int[2, 1] { { 1 }, { 2 } };
            var matrix2 = new Matrix(array2);

            Assert.IsFalse(matrix1.Equals(matrix2));
            Assert.IsTrue(matrix1.Equals(matrix1));
            Assert.IsFalse(matrix1.Equals(array1));
        }

        [Test]
        public void MatrixFromFileTest()
        {
            var matrix = new Matrix("Matrix.txt");
            Assert.AreEqual((2, 3), matrix.Size);

            var array = new int[2, 3] { { 1, 2, 3 }, { 4, 5, 10 } };
            var expected = new Matrix(array);
            Assert.AreEqual(expected, matrix);
        }

        [TestCase("EmptyFile.txt")]
        [TestCase("InvalidMatrix.txt")]
        public void InvalidMatrixFileExceptionTest(string filename)
            => Assert.Throws<InvalidMatrixFileException>(() => new Matrix(filename));

        [Test]
        public void WriteToFileTest()
        {
            var filename = "OutputMatrix.txt";
            var array = new int[2, 2] { { 1, 2 }, { 3, 4 } };
            var matrix = new Matrix(array);
            matrix.WriteToFile(filename);

            var expected = new List<string>() { "1 2 ", "3 4 " };
            var actual = new List<string>();
            using (var streamReader = new StreamReader(filename))
            {
                while (!streamReader.EndOfStream)
                {
                    actual.Add(streamReader.ReadLine());
                }
            }
            Assert.AreEqual(expected, actual);
        }
    }
}