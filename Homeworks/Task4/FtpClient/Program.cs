using System;
using System.Threading.Tasks;

namespace FtpClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Press Enter to connect.");
            Console.ReadLine();

            using var client = new Client("localhost", 8888);

            while (true)
            {
                Console.Write("Enter command: ");
                var command = Console.ReadLine();
                try
                {
                    switch (command)
                    {
                        case "1":
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

                        case "2":
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
                        case "exit":
                            {
                                return;
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
