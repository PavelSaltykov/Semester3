using FtpClient;
using Gui.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
            DownloadCommand = new AsyncCommand(async () =>
                {
                    filesToDownload.Add(SelectedServerItem.Path);
                    await DownloadSelectedFiles();
                }, () => SelectedServerItem != null && !SelectedServerItem.IsDir);

            DownloadAllCommand = new AsyncCommand(async () =>
                {
                    foreach (var item in FilesAndFolders)
                    {
                        if (!item.IsDir)
                        {
                            filesToDownload.Add(item.Path);
                        }
                    }
                    await DownloadSelectedFiles();
                }, () => FilesAndFolders.Any(item => !item.IsDir));

            NavigateToClientFolderCommand = new Command(NavigateToSelectedClientFolder, () => SelectedDownloadFolder != null);
            NavigateToSelectedClientFolder();
        }

        public bool IsDisconnected => client == null;

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
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AsyncCommand ConnectCommand { get; }

        private async Task Connect()
        {
            try
            {
                client = new Client(ip, port);
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
                    ClientFolders.Add(new FileSystemEntry("..", GetParentFolder(selectedFolder), true));
                }

                foreach (var item in entries)
                {
                    FilesAndFolders.Add(item);
                }
            }
            catch (IOException e)
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

        public ObservableCollection<(string, int)> Downloads { get; } = new ObservableCollection<(string, int)>();

        private List<string> filesToDownload = new List<string>();
        public AsyncCommand DownloadCommand { get; }
        public AsyncCommand DownloadAllCommand { get; }

        private async Task DownloadSelectedFiles()
        {
            //var res=await client.GetAsync(SelectedServerItem.Path);
        }

        private async Task DownloadFile(FileSystemEntry file, Stream sourceStream, long size)
        {
            string directoryPath = $@"{Directory.GetCurrentDirectory()}\{currentDownloadFolder}";
            using var fileStream = File.Create(@$"{directoryPath}\{file.Name}");

            const int maxBufferSize = 81920;
            var buffer = new byte[maxBufferSize];
            while (size > 0)
            {
                var currentBufferSize = size > maxBufferSize ? maxBufferSize : (int)size;
                await sourceStream.ReadAsync(buffer, 0, currentBufferSize);
                await fileStream.WriteAsync(buffer, 0, currentBufferSize);
                size -= maxBufferSize;
            }
        }

        public void Dispose() => client?.Dispose();
    }
}
