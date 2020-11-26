using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FtpClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.ReadLine();

            const int port = 8888;
            using var client = new TcpClient("localhost", port);

            using var stream = client.GetStream();
            using var writer = new StreamWriter(stream) { AutoFlush = true };
            using var reader = new StreamReader(stream);

            while (true)
            {
                Console.Write("Enter request: ");
                await writer.WriteLineAsync(Console.ReadLine());
                Console.WriteLine($"Response: {await reader.ReadLineAsync()}");
            }
        }
    }
}
