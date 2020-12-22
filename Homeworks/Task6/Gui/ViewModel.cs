using FtpClient;
using Gui.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private string ip = defaultIp;
        private int port = defaultPort;

        public ViewModel()
        {
            ConnectCommand = new ConnectCommand(Connect, () => client == null);
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
                currentServerFolder = @".\";
                var entries = await client.ListAsync(currentServerFolder);
                foreach(var item in entries)
                {
                    FilesAndFoldersOnServer.Add(item);
                }
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public ObservableCollection<FileSystemEntry> FilesAndFoldersOnServer { get; } = new ObservableCollection<FileSystemEntry>();
        private string currentServerFolder;

        public void Dispose() => client?.Dispose();
    }
}
