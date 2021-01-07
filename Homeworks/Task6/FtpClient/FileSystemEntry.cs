namespace FtpClient
{
    /// <summary>
    /// Represents information about a file system entry.
    /// </summary>
    public class FileSystemEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemEntry"/> class.
        /// </summary>
        /// <param name="name">Name of a file or a directory.</param>
        /// <param name="path">Path to the file system entry.</param>
        /// <param name="isDir">Value that indicates whether the entry is a directory.</param>
        public FileSystemEntry(string name, string path, bool isDir)
        {
            Name = name;
            Path = path;
            IsDir = isDir;
        }

        /// <summary>
        /// Gets the name of this file system entry.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the path to this file system entry.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets a value that indicates whether this entry is a directory.
        /// </summary>
        public bool IsDir { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is FileSystemEntry))
                return false;

            var other = obj as FileSystemEntry;
            return Name == other.Name && Path == other.Path && IsDir == other.IsDir;
        }

        public override int GetHashCode() => (Name, Path, IsDir).GetHashCode();
    }
}
