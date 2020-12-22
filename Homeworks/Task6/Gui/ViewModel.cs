using FtpClient;
using Gui.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
        private string ip = defaultIp;
        private int port = defaultPort;

        public ViewModel()
        {
            ConnectCommand = new AsyncCommand(Connect, () => client == null);
            NavigateToFolderCommand = new AsyncCommand(NavigateToSelectedFolder, () => SelectedServerItem.IsDir);
        }

        public string Ip
        {
            get => ip;
            set
            {
                ip = value;
                OnPropertyChanged(nameof(Ip));
            }
        }

        public string Port
        {
            get => port.ToString();
            set
            {
                port = int.Parse(value);
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
                await NavigateToSelectedFolder();
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private FileSystemEntry selectedServerItem;
        public FileSystemEntry SelectedServerItem
        {
            get => selectedServerItem ??= new FileSystemEntry("", rootFolder, true);
            set
            {
                selectedServerItem = value;
                OnPropertyChanged(nameof(SelectedServerItem));
            }
        }

        private const string rootFolder = ".";
        public ObservableCollection<FileSystemEntry> FilesAndFolders { get; } = new ObservableCollection<FileSystemEntry>();

        public AsyncCommand NavigateToFolderCommand { get; }

        private async Task NavigateToSelectedFolder()
        {
            try
            {
                string selectedFolder = SelectedServerItem.Path;
                var entries = await client.ListAsync(selectedFolder);

                FilesAndFolders.Clear();
                if (selectedFolder != rootFolder)
                {
                    var index = selectedFolder.LastIndexOf('\\');
                    var parentFolder = index > 0 ? selectedFolder.Remove(index) : rootFolder;
                    FilesAndFolders.Add(new FileSystemEntry("..", parentFolder, true));
                }

                foreach (var item in entries)
                {
                    FilesAndFolders.Add(item);
                }
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message);
                FilesAndFolders.Clear();
                client.Dispose();
                client = null;
            }
        }



        public void Dispose() => client?.Dispose();
    }
}
