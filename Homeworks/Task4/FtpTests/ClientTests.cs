using FtpClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ftp.Tests
{
    [TestFixture]
    public class ClientTests
    {
        private const int port = ServerSettings.Port;
        private Client client;

        [OneTimeSetUp]
        public void SetUp() => client = new Client("localhost", port);

        [Test]
        public void ListTest()
        {
            var response = client.ListAsync(ServerSettings.ListTestDir).Result;
            Assert.AreEqual(4, response.Count());

            var expectedResponse = new List<(string, bool)>
            {
                (new DirectoryInfo(ServerSettings.FolderPath1).FullName, true),
                (new DirectoryInfo(ServerSettings.FolderPath2).FullName, true),
                (new FileInfo(ServerSettings.FilePath1).FullName, false),
                (new FileInfo(ServerSettings.FilePath2).FullName, false)
            };

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
            new string[]{ ServerSettings.GetTestFilePath, "dowloadedFile.txt"},
            new string[]{ ServerSettings.GetTestBigFilePath, "dowloadedBigFile.txt"}
        };

        [TestCaseSource(nameof(pathsAndNamesForGetTest))]
        public void GetTest(string filePath, string downloadedFileName)
        {
            var downloadPath = @".\Dowlnoads";
            var dowloadedFilePath = $@"{downloadPath}\{downloadedFileName}";

            client.GetAsync(filePath, downloadPath, downloadedFileName).Wait();
            Assert.AreEqual(new FileInfo(filePath).Length, new FileInfo(dowloadedFilePath).Length);

            string expectedText;
            using (var fileReader = new StreamReader(filePath))
            {
                expectedText = fileReader.ReadToEnd();
            }

            using var downloadedFileReader = new StreamReader(dowloadedFilePath);
            Assert.AreEqual(expectedText, downloadedFileReader.ReadToEnd());
        }

        [OneTimeTearDown]
        public void TearDown() => client.Dispose();
    }
}
