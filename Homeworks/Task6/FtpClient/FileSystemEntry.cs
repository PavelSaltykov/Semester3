namespace FtpClient
{
    public class FileSystemEntry
    {
        public FileSystemEntry(string name, string path, bool isDir)
        {
            Name = name;
            Path = path;
            IsDir = isDir;
        }

        public string Name { get; }

        public string Path { get; }

        public bool IsDir { get; }
    }
}
