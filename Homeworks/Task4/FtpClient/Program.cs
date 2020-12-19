using System;
using System.Net;
using System.Threading.Tasks;

namespace FtpClient
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Please enter the IP address and port number of the remote server.");
                return 1;
            }

            var ip = args[0];
            if (!IPAddress.TryParse(ip, out _))
            {
                Console.WriteLine("Incorrect IP address.");
                return 1;
            }

            if (!int.TryParse(args[1], out var port))
            {
                Console.WriteLine("Incorrect port.");
                return 1;
            }

            Console.WriteLine("Press Enter to connect.");
            Console.ReadLine();

            using var client = new Client(ip, port);

            const string listingCommand = "1";
            const string downloadingCommand = "2";
            const string exitCommand = "exit";

            while (true)
            {
                Console.WriteLine("Enter command: ");
                Console.WriteLine($"  {listingCommand} - for listing files");
                Console.WriteLine($"  {downloadingCommand} - to download a file");
                Console.WriteLine($"  {exitCommand} - to exit");

                var command = Console.ReadLine();
                try
                {
                    switch (command)
                    {
                        case listingCommand:
                            {
                                Console.Write("Enter path: ");
                                var path = Console.ReadLine();
                                var response = await client.ListAsync(path);
                                Console.WriteLine("Response:");
                                foreach (var (name, isDir) in response)
                                {
                                    Console.WriteLine($"{name} {isDir}");
                                }
                                break;
                            }

                        case downloadingCommand:
                            {
                                Console.Write("Enter path: ");
                                var path = Console.ReadLine();

                                Console.Write("Enter destination path: ");
                                var destinationPath = Console.ReadLine();

                                Console.Write("Enter filename: ");
                                var filename = Console.ReadLine();

                                Console.WriteLine("Dowloading");
                                await client.GetAsync(path, destinationPath, filename);
                                Console.WriteLine("File downloaded");
                                break;
                            }
                        case exitCommand:
                            {
                                return 0;
                            }
                        default:
                            {
                                Console.WriteLine("Incorrect command.");
                                break;
                            }
                    }
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
