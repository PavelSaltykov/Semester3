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
    public class ViewModel : IDisposable, INotifyPropertyChanged
    {
        private const string defaultIp = "127.0.0.1";
        private const int defaultPort = 8888;
        private Client client;

        public ViewModel()
        {
            ConnectCommand = new AsyncCommand(Connect, () => IsDisconnected);
            NavigateToServerFolderCommand = new AsyncCommand(NavigateToSelectedServerFolder, () => SelectedServerItem.IsDir);
            DownloadCommand = new Command(() =>
               {
                   filesToDownload.Add(SelectedServerItem);
                   DownloadSelectedFiles();
               }, () => SelectedServerItem != null && !SelectedServerItem.IsDir);

            DownloadAllCommand = new Command(() =>
               {
                   foreach (var item in FilesAndFolders)
                   {
                       if (!item.IsDir)
                       {
                           filesToDownload.Add(item);
                       }
                   }
                   DownloadSelectedFiles();
               }, () => FilesAndFolders.Any(item => !item.IsDir));

            NavigateToClientFolderCommand = new Command(NavigateToSelectedClientFolder, () => SelectedDownloadFolder != null);
            NavigateToSelectedClientFolder();

            ClearCommand = new Command(ClearDownloads, () => Downloads.Count() > 0);
        }

        private bool isDisconnected = true;
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
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            MessageBox.Show(message);
            FilesAndFolders.Clear();
            client?.Dispose();
            client = null;
            IsDisconnected = true;
        }

        private FileSystemEntry selectedServerItem;
        public FileSystemEntry SelectedServerItem
        {
            get => selectedServerItem;
            set
            {
                selectedServerItem = value;
                OnPropertyChanged(nameof(SelectedServerItem));
            }
        }

        private const string rootFolder = ".";
        public ObservableCollection<FileSystemEntry> FilesAndFolders { get; } = new ObservableCollection<FileSystemEntry>();

        public AsyncCommand NavigateToServerFolderCommand { get; }

        private async Task NavigateToSelectedServerFolder()
        {
            try
            {
                string selectedFolder = SelectedServerItem?.Path ?? rootFolder;
                var entries = await client.ListAsync(selectedFolder);

                FilesAndFolders.Clear();
                if (selectedFolder != rootFolder)
                {
                    FilesAndFolders.Add(new FileSystemEntry("..", GetParentFolder(selectedFolder), true));
                }

                foreach (var item in entries)
                {
                    FilesAndFolders.Add(item);
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
        public string CurrentDownloadFolder
        {
            get => currentDownloadFolder;
            set
            {
                currentDownloadFolder = value;
                OnPropertyChanged(nameof(CurrentDownloadFolder));
            }
        }

        private FileSystemEntry selectedDownloadFolder;
        public FileSystemEntry SelectedDownloadFolder
        {
            get => selectedDownloadFolder ??= new FileSystemEntry(rootFolder, rootFolder, true);
            set
            {
                selectedDownloadFolder = value;
                OnPropertyChanged(nameof(SelectedDownloadFolder));
            }
        }

        public ObservableCollection<FileSystemEntry> ClientFolders { get; } = new ObservableCollection<FileSystemEntry>();

        public Command NavigateToClientFolderCommand { get; }

        private void NavigateToSelectedClientFolder()
        {
            var selectedFolder = SelectedDownloadFolder.Path;
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

        public ObservableCollection<DownloadFile> Downloads { get; } = new ObservableCollection<DownloadFile>();
        private readonly List<FileSystemEntry> filesToDownload = new List<FileSystemEntry>();

        public Command DownloadCommand { get; }
        public Command DownloadAllCommand { get; }

        private void DownloadSelectedFiles()
        {
            try
            {
                var startIndex = Downloads.Count;
                foreach (var file in filesToDownload)
                {
                    Downloads.Add(new DownloadFile(file));
                }
                var count = filesToDownload.Count;
                filesToDownload.Clear();
                Parallel.For(startIndex, count, async (i) =>
                {
                    var file = Downloads[i];
                    using var clientToDownload = new Client(Ip, Port);
                    var index = i;
                    Action<double> updatePercentage = p =>
                    {
                        if (index < Downloads.Count)
                        {
                            Downloads[index].Percentage = (int)p;
                        }
                    };
                    await clientToDownload.GetAsync(file.FileInfo.Path, currentDownloadFolder,
                         file.FileInfo.Name, updatePercentage);
                });
            }
            catch (IOException e)
            {
                if (e.InnerException is SocketException)
                {
                    HandleConnectionError(e.Message);
                    return;
                }
                MessageBox.Show(e.Message);
            }
        }

        public Command ClearCommand { get; }

        private void ClearDownloads() => Downloads.Clear();

        public void Dispose() => client?.Dispose();
    }
}
