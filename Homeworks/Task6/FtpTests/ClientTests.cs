using FtpClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FtpTests
{
    [TestFixture]
    public class ClientTests
    {
        private const int port = ServerSettings.Port;
        private Client client;
        private readonly string downloadPath = Path.Combine(".", "Downloads");

        [SetUp]
        public void SetUp() => client = new Client("localhost", port);

        public static List<FileSystemEntry> ExpectedList(string dirPath, bool needFiles)
        {
            var expectedList = new List<FileSystemEntry>();

            var folders = Directory.EnumerateDirectories(dirPath);
            foreach (var folder in folders)
            {
                var name = new DirectoryInfo(folder).Name;
                var path = Path.Combine(dirPath, name);
                expectedList.Add(new FileSystemEntry(name, path, true));
            }

            if (!needFiles)
                return expectedList;

            var files = Directory.EnumerateFiles(dirPath);
            foreach (var file in files)
            {
                var name = new FileInfo(file).Name;
                var path = Path.Combine(dirPath, name);
                expectedList.Add(new FileSystemEntry(name, path, false));
            }

            return expectedList;
        }

        [Test]
        public void ListTest()
        {
            var response = client.ListAsync(ServerSettings.ListTestDir).Result;
            Assert.AreEqual(4, response.Count());

            var expectedResponse = ExpectedList(ServerSettings.ListTestDir, true);

            CollectionAssert.AreEquivalent(expectedResponse, response);
        }

        [Test]
        public void NonexistentDirectoryListTest()
        {
            var path = $@"{ServerSettings.ServerDirectory}\NonexistentDirectory";
            var exception = Assert.Throws<AggregateException>(() => _ = client.ListAsync(path).Result);
            Assert.IsTrue(exception.InnerException is InvalidOperationException);
        }

        [Test]
        public void NonexistentFileGetTest()
        {
            var path = $@"{ServerSettings.ServerDirectory}\NonexistentFile.txt";
            var ex = Assert.Throws<AggregateException>(() => client.GetAsync(path, ".", "file.txt").Wait());
            Assert.IsTrue(ex.InnerException is InvalidOperationException);
        }

        private static readonly object[] pathsAndNamesForGetTest =
        {
            new string[]{ ServerSettings.GetTestFilePath, "downloadedFile.txt"},
            new string[]{ ServerSettings.GetTestBigFilePath, "downloadedBigFile.txt"}
        };

        [TestCaseSource(nameof(pathsAndNamesForGetTest))]
        public void GetTest(string filePath, string downloadedFileName)
        {
            var downloadedFilePath = $@"{downloadPath}\{downloadedFileName}";

            client.GetAsync(filePath, downloadPath, downloadedFileName).Wait();
            Assert.AreEqual(new FileInfo(filePath).Length, new FileInfo(downloadedFilePath).Length);

            string expectedText;
            using (var fileReader = new StreamReader(filePath))
            {
                expectedText = fileReader.ReadToEnd();
            }

            using var downloadedFileReader = new StreamReader(downloadedFilePath);
            Assert.AreEqual(expectedText, downloadedFileReader.ReadToEnd());
        }

        [TearDown]
        public void TearDown() => client.Dispose();

        [OneTimeTearDown]
        public void OneTimeTearDown() => Directory.Delete(downloadPath, true);
    }
}