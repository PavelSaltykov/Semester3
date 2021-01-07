using FtpServer;
using NUnit.Framework;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FtpTests
{
    [SetUpFixture]
    public class ServerSettings
    {
        public const int Port = 8000;
        private Server server;

        public static readonly string ServerDirectory = Path.Combine(".", "ServerDirectory");

        public static readonly string ListTestDir = $@"{ServerDirectory}\ListTest";
        public static readonly string FolderPath1 = $@"{ListTestDir}\Folder1";
        public static readonly string FolderPath2 = $@"{ListTestDir}\Folder2";
        public static readonly string FilePath1 = $@"{ListTestDir}\File1.txt";
        public static readonly string FilePath2 = $@"{ListTestDir}\File2.txt";

        public static readonly string EmptyDir = $@"{ServerDirectory}\EmptyDirectory";

        public static readonly string GetTestDir = $@"{ServerDirectory}\GetTest";
        public static readonly string GetTestFilePath = $@"{GetTestDir}\file.txt";
        public static readonly string GetTestBigFilePath = $@"{GetTestDir}\bigFile.txt";
        public static readonly string DownloadTestFilePath = $@"{GetTestDir}\downloadFile.txt";

        public static readonly string DownloadAllDir = $@"{ServerDirectory}\DownloadAllTest";
        public static readonly string DownloadAllTestFile1 = $@"{DownloadAllDir}\file1.txt";
        public static readonly string DownloadAllTestFile2 = $@"{DownloadAllDir}\file2.txt";

        [OneTimeSetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(ServerDirectory);

            Directory.CreateDirectory(FolderPath1);
            Directory.CreateDirectory(FolderPath2);
            File.Create(FilePath1).Close();
            File.Create(FilePath2).Close();

            Directory.CreateDirectory(EmptyDir);

            Directory.CreateDirectory(GetTestDir);
            CreateFile("Text in the file.", GetTestFilePath);
            CreateFile(GetBigText(), GetTestBigFilePath);
            CreateFile("Download test.", DownloadTestFilePath);

            Directory.CreateDirectory(DownloadAllDir);
            File.Create(DownloadAllTestFile1).Close();
            File.Create(DownloadAllTestFile2).Close();

            server = new Server(IPAddress.Loopback, Port);
            _ = Task.Run(async () => await server.Run());
        }

        private void CreateFile(string text, string filePath)
        {
            byte[] bytes = new UTF8Encoding(true).GetBytes(text);
            using var fileStream = File.Create(filePath);
            fileStream.Write(bytes);
        }

        private string GetBigText()
        {
            var text = "";
            for (var i = 0; i < 100000; ++i)
            {
                text += "a";
            }
            return text;
        }

        [OneTimeTearDown]
        public void TearDown() => Directory.Delete(ServerDirectory, true);
    }
}