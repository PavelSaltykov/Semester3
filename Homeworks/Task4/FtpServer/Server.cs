using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FtpServer
{
    public class Server
    {
        private readonly TcpListener listener;

        public Server(IPAddress localAddr, int port) => listener = new TcpListener(localAddr, port);

        public async Task Start()
        {
            listener.Start();
            while (true)
            {
                var socket = await listener.AcceptSocketAsync();
                var stream = new NetworkStream(socket);
                try
                {
                    _ = Task.Run(async () => await HandleRequests(stream));
                }
                catch
                {
                    stream.Close();
                    socket.Close();
                }
            }
        }

        private async Task HandleRequests(NetworkStream stream)
        {
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

                var path = request[2..];
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
                response += $" {directory} true";
            }
            foreach (var file in files)
            {
                response += $" {file} false";
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
        }
    }
}
