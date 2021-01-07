using NUnit.Framework;
using System.IO;
using System.Net.Sockets;

namespace FtpTests
{
    [TestFixture]
    public class ServerTests
    {
        private const int port = ServerSettings.Port;
        private TcpClient tcpClient;

        private StreamReader reader;
        private StreamWriter writer;

        [OneTimeSetUp]
        public void SetUp()
        {
            tcpClient = new TcpClient("localhost", port);
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream()) { AutoFlush = true };
        }

        [TestCase("")]
        [TestCase("1")]
        [TestCase("2.")]
        [TestCase(@"3 .\ServerDirectory")]
        public void IncorrectRequestTest(string request)
        {
            const string errorMessage = "Incorrect request.";
            writer.WriteLine(request);
            Assert.AreEqual(reader.ReadLine(), errorMessage);
        }

        [TestCase(@"1 .\Test")]
        [TestCase(@"1 .\ServerDirectory\NonexistentDirectory")]
        public void NonexistentDirectoryListTest(string request)
        {
            writer.WriteLine(request);
            Assert.AreEqual(reader.ReadLine(), "-1");
        }

        [Test]
        public void ListTest()
        {
            writer.WriteLine($"1 {ServerSettings.ListTestDir}");
            var response = reader.ReadLine().Split(' ');
            var actualResponse = new string[]
                {
                    response[0],
                    $"{response[1]} {response[2]}",
                    $"{response[3]} {response[4]}",
                    $"{response[5]} {response[6]}",
                    $"{response[7]} {response[8]}",
                };

            var expected = new string[]
                {
                    "4",
                    $"{ServerSettings.FolderPath1} true",
                    $"{ServerSettings.FolderPath2} true",
                    $"{ServerSettings.FilePath1} false",
                    $"{ServerSettings.FilePath2} false"
                };
            CollectionAssert.AreEquivalent(expected, actualResponse);
        }

        [Test]
        public void EmptyDirectoryTest()
        {
            writer.WriteLine($"1 {ServerSettings.EmptyDir}");
            Assert.AreEqual(reader.ReadLine(), "0");
        }

        [Test]
        public void GetTest()
        {
            var filePath = ServerSettings.GetTestFilePath;
            writer.WriteLine($"2 {filePath}");
            var response = reader.ReadLine();

            string textInFile;
            using (var fileReader = new StreamReader(filePath))
            {
                textInFile = fileReader.ReadToEnd();
            }
            var expectedResponse = $"{new FileInfo(filePath).Length} {textInFile}";

            Assert.AreEqual(expectedResponse, response);
        }

        [Test]
        public void GetNonexistentFileTest()
        {
            var filePath = $@"{ServerSettings.ServerDirectory}\NonexistentFile.txt";
            writer.WriteLine($"2 {filePath}");

            var response = reader.ReadLine();
            Assert.AreEqual("-1", response);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            reader.Dispose();
            writer.Dispose();
            tcpClient.Dispose();
        }
    }
}