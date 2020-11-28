using System.Net;
using System.Threading.Tasks;

namespace FtpServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var server = new Server(IPAddress.Loopback, 8888);
            await server.Run();
        }
    }
}
