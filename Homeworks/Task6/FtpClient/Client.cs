using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FtpClient
{
    /// <summary>
    /// FTP client that allows to make requests:
    /// listing files in a directory on the server
    /// and downloading a file from the server.
    /// </summary>
    public class Client : IDisposable
    {
        private readonly TcpClient client;
        private readonly Stream stream;
        private readonly StreamWriter writer;
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class and connects
        /// to the specified port on the specified host.
        /// </summary>
        /// <param name="hostname">The DNS name of the remote host to which you intend to connect.</param>
        /// <param name="port">The port number of the remote host to which you intend to connect.</param>
        public Client(string hostname, int port)
        {
            client = new TcpClient(hostname, port);
            stream = client.GetStream();
            writer = new StreamWriter(stream) { AutoFlush = true };
            reader = new StreamReader(stream);
        }

        /// <summary>
        /// Sends a request to listing files in a directory on the server.
        /// </summary>
        /// <param name="path">Path to directory relative to where the server is running.</param>
        /// <returns>Names of folders and files in the directory on the server.</returns>
        public async Task<IEnumerable<FileSystemEntry>> ListAsync(string path)
        {
            await writer.WriteLineAsync($"1 {path}");
            var response = await reader.ReadLineAsync();
            if (response == "-1")
                throw new InvalidOperationException($"Directory not found at path: {path}.");

            var splittedResponse = response.Split(' ');
            var size = int.Parse(splittedResponse[0]);

            if (splittedResponse.Length != 1 + size * 2)
                throw new InvalidOperationException("Incorrect response.");

            var result = new List<FileSystemEntry>();
            for (var i = 0; i < size; ++i)
            {
                var entryPath = splittedResponse[i * 2 + 1];
                var isDir = bool.Parse(splittedResponse[i * 2 + 2]);
                var name = entryPath.Split('\\', '/').LastOrDefault();
                result.Add(new FileSystemEntry(name, entryPath, isDir));
            }
            return result;
        }

        /// <summary>
        /// Sends a request to download a file from a directory on the server
        /// and saves the file to the client folder.
        /// </summary>
        /// <param name="path">Path to file relative to where the server is running.</param>
        /// <param name="destinationPath">Path to download folder relative to where the client is running.</param>
        /// <param name="filename">Name of downloaded file.</param>
        public async Task GetAsync(string path, string destinationPath, string filename, Action<double> updatePercentage = null)
        {
            await writer.WriteLineAsync($"2 {path}");

            var size = new char[long.MaxValue.ToString().Length + 1];
            await reader.ReadAsync(size, 0, 2);
            if (size[0] == '-')
                throw new InvalidOperationException($"File not found at path: {path}");

            var index = 1;
            while (size[index] != ' ')
            {
                index++;
                await reader.ReadAsync(size, index, 1);
            }

            await Download(long.Parse(size), destinationPath, filename, updatePercentage);
        }

        private async Task Download(long size, string path, string filename, Action<double> updatePercentage)
        {
            string directoryPath = $@"{Directory.GetCurrentDirectory()}\{path}";
            using var fileStream = File.Create(@$"{directoryPath}\{filename}");

            const int maxBufferSize = 81920;
            var buffer = new byte[maxBufferSize];

            var needToDownload = size;
            var increaseInPercentage = size != 0 ? (float)maxBufferSize / size * 100 : 100;
            float percentage = 0;
            while (needToDownload > 0)
            {
                var currentBufferSize = needToDownload > maxBufferSize ? maxBufferSize : (int)needToDownload;
                await stream.ReadAsync(buffer, 0, currentBufferSize);
                await fileStream.WriteAsync(buffer, 0, currentBufferSize);

                percentage += increaseInPercentage;
                percentage = percentage < 100 ? percentage : 100;
                updatePercentage?.Invoke(percentage);
                needToDownload -= maxBufferSize;
            }
            updatePercentage?.Invoke(100);
            await reader.ReadLineAsync();
        }

        public void Dispose()
        {
            writer.Dispose();
            reader.Dispose();
            stream.Dispose();
            client.Dispose();
        }
    }
}
