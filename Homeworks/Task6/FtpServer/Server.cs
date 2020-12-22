using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FtpServer
{
    /// <summary>
    /// FTP server processing two requests: 
    /// listing files in a directory on the server
    /// and downloading a file from the server.
    /// </summary>
    public class Server
    {
        private readonly TcpListener listener;
        public string RootDirectory => @$"{Directory.GetCurrentDirectory()}\Root";

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class
        /// and starts listening for incoming connection requests. 
        /// </summary>
        /// <param name="localAddr"><see cref="IPAddress"/> that represents the local IP address.</param>
        /// <param name="port">The port on which to listen for incoming connection attempts.</param>
        public Server(IPAddress localAddr, int port)
        {
            Directory.CreateDirectory(RootDirectory);
            listener = new TcpListener(localAddr, port);
            listener.Start();
        }

        /// <summary>
        /// Starts processing requests.
        /// </summary>
        public async Task Run()
        {
            while (true)
            {
                var socket = await listener.AcceptSocketAsync();
                _ = Task.Run(async () => await ProcessRequests(socket));
            }
        }

        private async Task ProcessRequests(Socket socket)
        {
            using (socket)
            {
                using var stream = new NetworkStream(socket);
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream) { AutoFlush = true };

                while (true)
                {
                    var request = await reader.ReadLineAsync();

                    if (!Regex.IsMatch(request, @"^[12]\s.+"))
                    {
                        await writer.WriteLineAsync("Incorrect request.");
                        continue;
                    }

                    var path = @$"{RootDirectory}\{request[2..]}\";
                    switch (request[0])
                    {
                        case '1':
                            {
                                await List(path, writer);
                                break;
                            }
                        case '2':
                            {
                                await Get(path, writer);
                                break;
                            }
                    }
                }
            }
        }

        private async Task List(string path, StreamWriter writer)
        {
            if (!Directory.Exists(path))
            {
                await writer.WriteLineAsync("-1");
                return;
            }

            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            var response = (directories.Count() + files.Count()).ToString();

            foreach (var directory in directories)
            {
                var dirPath = directory.Remove(0, directory.IndexOf(@".\"));
                response += $" {dirPath} true";
            }
            foreach (var file in files)
            {
                var filePath = file.Remove(0, file.IndexOf(@".\"));
                response += $" {filePath} false";
            }
            await writer.WriteLineAsync(response);
        }

        private async Task Get(string path, StreamWriter writer)
        {
            if (!File.Exists(path))
            {
                await writer.WriteLineAsync("-1");
                return;
            }

            await writer.WriteAsync($"{new FileInfo(path).Length} ");
            using var fileStream = File.OpenRead(path);
            await fileStream.CopyToAsync(writer.BaseStream);
            await writer.WriteLineAsync();
        }
    }
}
