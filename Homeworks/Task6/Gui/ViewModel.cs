using FtpClient;
using Gui.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            ConnectCommand = new ConnectCommand(Connect, () => client == null);
            NavigateToFolderCommand = new ListItemCommand(async i =>
                await NavigateToSelectedFolder(FilesAndFolders[i].Path), i => FilesAndFolders[i].IsDir);
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

        public ConnectCommand ConnectCommand { get; }

        private async Task Connect()
        {
            try
            {
                client = new Client(ip, port);
                await NavigateToSelectedFolder(rootFolder);
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private const string rootFolder = ".";
        public ObservableCollection<FileSystemEntry> FilesAndFolders { get; } = new ObservableCollection<FileSystemEntry>();

        public ListItemCommand NavigateToFolderCommand { get; }

        private async Task NavigateToSelectedFolder(string selectedFolder)
        {
            var entries = await client.ListAsync(selectedFolder);

            FilesAndFolders.Clear();
            if (selectedFolder != rootFolder)
            {
                var parentFolder = selectedFolder.Remove(selectedFolder.LastIndexOf('\\'));
                FilesAndFolders.Add(new FileSystemEntry("..", parentFolder, true));
            }

            foreach (var item in entries)
            {
                FilesAndFolders.Add(item);
            }
        }

        public void Dispose() => client?.Dispose();
    }
}
