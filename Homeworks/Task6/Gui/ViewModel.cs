using FtpClient;
using Gui.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace Gui
{
    /// <summary>
    /// View Model for the main window.
    /// </summary>
    public class ViewModel : IDisposable, INotifyPropertyChanged
    {
        private const string defaultIp = "127.0.0.1";
        private const int defaultPort = 8888;
        private Client client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            ConnectCommand = new AsyncCommand(Connect, () => IsDisconnected);

            NavigateToServerFolderCommand = new AsyncCommand(NavigateToSelectedServerFolder, () => SelectedServerItem.IsDir);
            NavigateToClientFolderCommand = new Command(NavigateToSelectedClientFolder, () => SelectedClientFolder != null);
            NavigateToSelectedClientFolder();

            DownloadCommand = new AsyncCommand(async () =>
               {
                   filesToDownload.Add(SelectedServerItem);
                   await DownloadSelectedFiles();
               }, () => SelectedServerItem != null && !SelectedServerItem.IsDir);

            DownloadAllCommand = new AsyncCommand(async () =>
               {
                   foreach (var item in ServerFoldersAndFiles)
                   {
                       if (!item.IsDir)
                       {
                           filesToDownload.Add(item);
                       }
                   }
                   await DownloadSelectedFiles();
               }, () => ServerFoldersAndFiles.Any(item => !item.IsDir));

            ClearCommand = new Command(ClearDownloads, () => Downloads.Count() > 0);
        }

        private bool isDisconnected = true;

        /// <summary>
        /// Gets or sets value that indicates whether the client is not connected to the server.
        /// </summary>
        public bool IsDisconnected
        {
            get => isDisconnected;
            set
            {
                isDisconnected = value;
                OnPropertyChanged(nameof(IsDisconnected));
            }
        }

        private string ip = defaultIp;

        /// <summary>
        /// Ip address to connect to the server.
        /// </summary>
        public string Ip
        {
            get => ip;
            set
            {
                ip = value;
                OnPropertyChanged(nameof(Ip));
            }
        }

        private int port = defaultPort;

        /// <summary>
        /// Port to connect to the server.
        /// </summary>
        public int Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Command to connect to the server.
        /// </summary>
        public AsyncCommand ConnectCommand { get; }

        private async Task Connect()
        {
            try
            {
                client = new Client(ip, port);
                IsDisconnected = false;
                await NavigateToSelectedServerFolder();
            }
            catch (SocketException e)
            {
                HandleConnectionError(e.Message);
            }
        }

        private void HandleConnectionError(string message)
        {
            client?.Dispose();
            client = null;
            IsDisconnected = true;
            MessageBox.Show(message);
            ServerFoldersAndFiles.Clear();
        }

        private FileSystemEntry selectedServerItem;

        /// <summary>
        /// Selected item in the list of server files and folders.
        /// </summary>
        public FileSystemEntry SelectedServerItem
        {
            get => selectedServerItem;
            set
            {
                selectedServerItem = value;
                OnPropertyChanged(nameof(SelectedServerItem));
            }
        }

        /// <summary>
        /// List of server files and folders.
        /// </summary>
        public ObservableCollection<FileSystemEntry> ServerFoldersAndFiles { get; } = new ObservableCollection<FileSystemEntry>();

        private const string rootFolder = ".";

        /// <summary>
        /// Command to navigate to the selected server folder.
        /// </summary>
        public AsyncCommand NavigateToServerFolderCommand { get; }

        private async Task NavigateToSelectedServerFolder()
        {
            try
            {
                string selectedFolder = SelectedServerItem?.Path ?? rootFolder;
                var entries = await client.ListAsync(selectedFolder);

                ServerFoldersAndFiles.Clear();
                if (selectedFolder != rootFolder)
                {
                    ServerFoldersAndFiles.Add(new FileSystemEntry("..", GetParentFolder(selectedFolder), true));
                }

                foreach (var item in entries)
                {
                    ServerFoldersAndFiles.Add(item);
                }
            }
            catch (IOException e) when (e.InnerException is SocketException)
            {
                HandleConnectionError(e.Message);
            }
        }

        private static string GetParentFolder(string path)
        {
            var index = path.LastIndexOfAny(new[] { '\\', '/' });
            return path.Remove(index);
        }

        private string currentDownloadFolder;

        /// <summary>
        /// Current selected client folder to download.
        /// </summary>
        public string CurrentDownloadFolder
        {
            get => currentDownloadFolder;
            private set
            {
                currentDownloadFolder = value;
                OnPropertyChanged(nameof(CurrentDownloadFolder));
            }
        }

        private FileSystemEntry selectedClientFolder;

        /// <summary>
        /// Selected folder in the list of client folders.
        /// </summary>
        public FileSystemEntry SelectedClientFolder
        {
            get => selectedClientFolder ??= new FileSystemEntry(rootFolder, rootFolder, true);
            set
            {
                selectedClientFolder = value;
                OnPropertyChanged(nameof(SelectedClientFolder));
            }
        }

        /// <summary>
        /// List of client folders.
        /// </summary>
        public ObservableCollection<FileSystemEntry> ClientFolders { get; } = new ObservableCollection<FileSystemEntry>();

        /// <summary>
        /// Command to navigate to the selected client folder.
        /// </summary>
        public Command NavigateToClientFolderCommand { get; }

        private void NavigateToSelectedClientFolder()
        {
            var selectedFolder = SelectedClientFolder.Path;
            var folders = Directory.EnumerateDirectories(selectedFolder);
            ClientFolders.Clear();
            if (selectedFolder != rootFolder)
            {
                ClientFolders.Add(new FileSystemEntry("..", GetParentFolder(selectedFolder), true));
            }

            foreach (var item in folders)
            {
                var name = item.Split('\\', '/').LastOrDefault();
                ClientFolders.Add(new FileSystemEntry(name, item, true));
            }
            CurrentDownloadFolder = selectedFolder;
        }

        /// <summary>
        /// List of downloads.
        /// </summary>
        public ObservableCollection<DownloadFile> Downloads { get; } = new ObservableCollection<DownloadFile>();
        private readonly List<FileSystemEntry> filesToDownload = new List<FileSystemEntry>();

        /// <summary>
        /// Command to download the selected server file.
        /// </summary>
        public AsyncCommand DownloadCommand { get; }

        /// <summary>
        /// Command to download all files in the current server folder.
        /// </summary>
        public AsyncCommand DownloadAllCommand { get; }

        private async Task DownloadSelectedFiles()
        {
            var startIndex = Downloads.Count;
            foreach (var file in filesToDownload)
            {
                Downloads.Add(new DownloadFile(file));
            }
            var count = filesToDownload.Count;
            filesToDownload.Clear();
            await ParallelDownload(startIndex, count);
        }

        private readonly object lockObject = new object();

        private async Task ParallelDownload(int startIndex, int count)
        {
            try
            {
                var tasks = new List<Task>();
                for (var i = startIndex; i < startIndex + count; ++i)
                {
                    var index = i;
                    tasks.Add(Task.Run(async () => await DownloadFile(index)));
                }
                await Task.WhenAll(tasks);
            }
            catch (IOException e)
            {
                lock (lockObject)
                {
                    if (!isDisconnected)
                    {
                        HandleConnectionError(e.InnerException.Message);
                    }
                }
            }
        }

        private async Task DownloadFile(int index)
        {
            var file = Downloads[index];
            using var clientToDownload = new Client(Ip, Port);
            Action<double> updatePercentage = p =>
            {
                if (index < Downloads.Count && ReferenceEquals(file, Downloads[index]))
                {
                    Downloads[index].Percentage = (int)p;
                }
            };
            try
            {
                await clientToDownload.GetAsync(file.FileInfo.Path, currentDownloadFolder,
                     file.FileInfo.Name, updatePercentage);
            }
            catch (IOException e) when (!(e.InnerException is SocketException))
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Command to clear list of downloads.
        /// </summary>
        public Command ClearCommand { get; }

        private void ClearDownloads() => Downloads.Clear();

        public void Dispose() => client?.Dispose();
    }
}
