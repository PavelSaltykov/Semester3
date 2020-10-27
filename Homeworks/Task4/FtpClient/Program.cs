using System;
using System.IO;
using System.Net.Sockets;

namespace FtpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("FtpClient");
            const int port = 8888;

            using var client = new TcpClient("localhost", port);
            using var stream = client.GetStream();
            using var writer = new StreamWriter(stream) { AutoFlush = true };

            var message = Console.ReadLine();
            writer.WriteLine(message);

            using var reader = new StreamReader(stream);
            var data = reader.ReadLine();
            Console.WriteLine($"Received: {data}");
        }
    }
}
