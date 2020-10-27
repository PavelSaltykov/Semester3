using System;
using System.Net;
using System.Threading.Tasks;

namespace FtpServer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("FtpServer");
            const int port = 8888;
            var server = new Server(IPAddress.Loopback, port);
            await server.Start();
        }
    }
}
