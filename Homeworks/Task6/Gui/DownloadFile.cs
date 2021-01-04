using FtpClient;
using System.ComponentModel;

namespace Gui
{
    public class DownloadFile : INotifyPropertyChanged
    {
        public DownloadFile(FileSystemEntry fileInfo) => FileInfo = fileInfo;

        public FileSystemEntry FileInfo { get; }

        private int percentage;
        public int Percentage
        {
            get => percentage;
            set
            {
                percentage = value;
                OnPropertyChanged(nameof(Percentage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
