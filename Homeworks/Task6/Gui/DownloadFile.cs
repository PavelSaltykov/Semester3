using FtpClient;
using System.ComponentModel;

namespace Gui
{
    /// <summary>
    /// Represents information about a file for download.
    /// </summary>
    public class DownloadFile : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFile"/> class.
        /// </summary>
        /// <param name="fileInfo">Info about the file.</param>
        public DownloadFile(FileSystemEntry fileInfo) => FileInfo = fileInfo;

        /// <summary>
        /// Information about this file.
        /// </summary>
        public FileSystemEntry FileInfo { get; }

        private int percentage;

        /// <summary>
        /// Gets or sets download percentage of this file.
        /// </summary>
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
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
