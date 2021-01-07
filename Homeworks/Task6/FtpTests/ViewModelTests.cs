using FtpClient;
using Gui;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace FtpTests
{
    [TestFixture]
    public class ViewModelTests
    {
        private const int port = ServerSettings.Port;
        private ViewModel vm;
        private readonly string downloadDir = Path.Combine(Directory.GetDirectoryRoot("."), "DownloadsVM");

        [OneTimeSetUp]
        public void OneTimeSetUp() => Directory.CreateDirectory(downloadDir);

        [SetUp]
        public void SetUp()
        {
            vm = new ViewModel { Port = port };
            vm.ConnectCommand.ExecuteAsync().Wait();
        }

        [Test]
        public void ConnectTest()
        {
            var expectedList = ClientTests.ExpectedList(".", true);

            CollectionAssert.AreEquivalent(expectedList, vm.ServerFoldersAndFiles);
        }

        private FileSystemEntry CreateFileSystemEntry(string path, bool isDir)
        {
            var dirName = new DirectoryInfo(path).Name;
            return new FileSystemEntry(dirName, path, isDir);
        }

        private void NavigateToServerFolder(string path)
        {
            var dir = CreateFileSystemEntry(path, true);
            var index = vm.ServerFoldersAndFiles.IndexOf(dir);
            vm.SelectedServerItem = vm.ServerFoldersAndFiles[index];
            vm.NavigateToServerFolderCommand.ExecuteAsync().Wait();
        }

        private void NavigateToClientFolder(string path)
        {
            var dir = CreateFileSystemEntry(path, true);
            var index = vm.ClientFolders.IndexOf(dir);
            vm.SelectedClientFolder = vm.ClientFolders[index];
            vm.NavigateToClientFolderCommand.Execute();
        }

        [Test]
        public void NavigateToServerFolderTest()
        {
            var path = ServerSettings.ServerDirectory;
            NavigateToServerFolder(path);
            var expectedList = ClientTests.ExpectedList(path, true);
            expectedList.Insert(0, new FileSystemEntry("..", ".", true));

            CollectionAssert.AreEquivalent(expectedList, vm.ServerFoldersAndFiles);
        }

        [Test]
        public void NavigateToClientFolderTest()
        {
            NavigateToClientFolder(downloadDir);
            Assert.AreEqual(downloadDir, vm.CurrentDownloadFolder);

            var expectedList = ClientTests.ExpectedList(downloadDir, false);
            expectedList.Insert(0, new FileSystemEntry("..", Directory.GetDirectoryRoot("."), true));

            CollectionAssert.AreEquivalent(expectedList, vm.ClientFolders);
        }

        [Test]
        public void DownloadTest()
        {
            NavigateToClientFolder(downloadDir);

            var path1 = ServerSettings.ServerDirectory;
            NavigateToServerFolder(path1);
            var path2 = ServerSettings.GetTestDir;
            NavigateToServerFolder(path2);

            var file = CreateFileSystemEntry(ServerSettings.DownloadTestFilePath, false);
            var index = vm.ServerFoldersAndFiles.IndexOf(file);
            vm.SelectedServerItem = vm.ServerFoldersAndFiles[index];

            Assert.IsTrue(vm.DownloadCommand.CanExecute());
            vm.DownloadCommand.ExecuteAsync().Wait();

            var downloadedFilePath = Path.Combine(downloadDir, file.Name);
            Assert.AreEqual(new FileInfo(file.Path).Length, new FileInfo(downloadedFilePath).Length);

            string expectedText;
            using (var fileReader = new StreamReader(file.Path))
            {
                expectedText = fileReader.ReadToEnd();
            }

            using var downloadedFileReader = new StreamReader(downloadedFilePath);
            Assert.AreEqual(expectedText, downloadedFileReader.ReadToEnd());
        }

        [Test]
        public void DownloadAllTest()
        {
            NavigateToClientFolder(downloadDir);

            var path1 = ServerSettings.ServerDirectory;
            NavigateToServerFolder(path1);
            var path2 = ServerSettings.DownloadAllDir;
            NavigateToServerFolder(path2);

            Assert.IsTrue(vm.DownloadAllCommand.CanExecute());
            vm.DownloadAllCommand.ExecuteAsync().Wait();

            var files = vm.ServerFoldersAndFiles.Where(e => !e.IsDir);

            foreach (var file in files)
            {
                var downloadedFilePath = Path.Combine(downloadDir, file.Name);
                Assert.IsTrue(File.Exists(downloadedFilePath));
                Assert.AreEqual(new FileInfo(file.Path).Length, new FileInfo(downloadedFilePath).Length);
            }
        }

        [TearDown]
        public void TearDown() => vm.Dispose();

        [OneTimeTearDown]
        public void OneTimeTearDown() => Directory.Delete(downloadDir, true);
    }
}