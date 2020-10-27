using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FtpServer
{
    public class Server
    {
        private readonly TcpListener listener;

        public Server(IPAddress localAddr, int port)
        {
            listener = new TcpListener(localAddr, port);
        }

        public async Task Start()
        {
            listener.Start();
            Console.WriteLine("Server is running");
            while (true)
            {
                var socket = await listener.AcceptSocketAsync();
                Console.WriteLine("Connected");
                Handle(socket);
            }
        }

        private async void Handle(Socket socket)
        {
            using var stream = new NetworkStream(socket);
            using var reader = new StreamReader(stream);

            var data = await reader.ReadLineAsync();
            Console.WriteLine($"Received: {data}");

            using var writer = new StreamWriter(stream) { AutoFlush = true };

            await writer.WriteAsync("Hi!");

            socket.Close();
        }
    }
}
