using NUnit.Framework;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ftp.Tests
{
    [TestFixture]
    public class ServerTests
    {
        private const int port = 8888;
        private Server server;
        private TcpClient tcpClient;

        private StreamReader reader;
        private StreamWriter writer;
        private readonly string ServerDirectoryPath = Directory.GetCurrentDirectory() + @"\ServerDirectory";

        [OneTimeSetUp]
        public void Setup()
        {
            server = new Server(IPAddress.Loopback, port);
            _ = Task.Run(async () => await server.Start());

            tcpClient = new TcpClient("localhost", port);
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream()) { AutoFlush = true };

            Directory.CreateDirectory(ServerDirectoryPath);
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
        [TestCase(@"1 .\ServerDirectory\Files")]
        public void NonexistentDirectoryListTest(string request)
        {
            writer.WriteLine(request);
            Assert.AreEqual(reader.ReadLine(), "-1");
        }

        [Test]
        public void ListTest()
        {
            var testDirectory = $@"{ServerDirectoryPath}\ListTest";

            var folderPath1 = $@"{testDirectory}\Folder1";
            var folderPath2 = $@"{testDirectory}\Folder2";
            var filePath1 = $@"{testDirectory}\File1.txt";
            var filePath2 = $@"{testDirectory}\File2.txt";

            Directory.CreateDirectory(folderPath1);
            Directory.CreateDirectory(folderPath2);
            File.Create(filePath1).Close();
            File.Create(filePath2).Close();

            writer.WriteLine($"1 {testDirectory}");
            var response = (reader.ReadLine()).Split(' ');
            var actualResponse = new string[]
                {
                    response[0],
                    $"{response[1]} {response[2]}",
                    $"{response[3]} {response[4]}",
                    $"{response[5]} {response[6]}",
                    $"{response[7]} {response[8]}",
                };

            var expectedResponse = new string[]
                {
                    "4",
                    $"{folderPath1} true",
                    $"{folderPath2} true",
                    $"{filePath1} false",
                    $"{filePath2} false"
                };
            CollectionAssert.AreEquivalent(expectedResponse, actualResponse);
            Directory.Delete(testDirectory, true);
        }

        [Test]
        public void EmptyDirectoryTest()
        {
            var emptyDirectoryPath = $@"{ServerDirectoryPath}\EmptyDirectory";
            Directory.CreateDirectory(emptyDirectoryPath);

            writer.WriteLine($"1 {emptyDirectoryPath}");
            Assert.AreEqual(reader.ReadLine(), "0");

            Directory.Delete(emptyDirectoryPath);
        }

        [Test]
        public void GetTest()
        {
            var testDirectory = $@"{ServerDirectoryPath}\File";
            Directory.CreateDirectory(testDirectory);

            var filePath = $@"{testDirectory}\file.txt";
            var textInFile = "Text in the file.";
            byte[] text = new UTF8Encoding(true).GetBytes(textInFile);
            using (var fileStream = File.Create(filePath))
            {
                fileStream.Write(text);
            }

            writer.WriteLine($"2 {filePath}");
            var response = reader.ReadLine();

            var expectedResponse = $"{text.Length} {textInFile}";
            Assert.AreEqual(expectedResponse, response);

            Directory.Delete(testDirectory, true);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            reader.Dispose();
            writer.Dispose();
            tcpClient.Dispose();
            Directory.Delete(ServerDirectoryPath, true);
        }
    }
}