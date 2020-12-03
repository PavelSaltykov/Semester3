using System;
using System.Net;
using System.Net.Sockets;

namespace Test1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 && args.Length != 2)
            {
                Console.WriteLine("Incorrect command line arguments");
                return;
            }

            try
            {
                if (args.Length == 1)
                {
                    if (int.TryParse(args[0], out var port))
                    {
                        var server = new Server(port);
                        server.Start();
                    }
                    else
                    {
                        Console.WriteLine("Incorrect command line arguments");
                    }
                }
                else
                {
                    if (IPAddress.TryParse(args[0], out _) && int.TryParse(args[1], out var port))
                    {
                        using var client = new Client(args[0], port);
                        client.Start();
                    }
                    else
                    {
                        Console.WriteLine("Incorrect command line arguments");
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }
    }
}
