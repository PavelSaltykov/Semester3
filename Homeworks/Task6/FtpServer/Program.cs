﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FtpServer
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Please enter the IP address and port number.");
                return 1;
            }

            if (!IPAddress.TryParse(args[0], out var ip))
            {
                Console.WriteLine("Incorrect IP address.");
                return 1;
            }

            if (!int.TryParse(args[1], out var port))
            {
                Console.WriteLine("Incorrect port.");
                return 1;
            }

            var server = new Server(ip, port);

            Directory.CreateDirectory(@$"{server.RootDirectory}\Folder");
            Directory.CreateDirectory(@$"{server.RootDirectory}\Folder\Subfolder");
            File.Create(@$"{server.RootDirectory}\Folder\file.txt");

            await server.Run();
            return 0;
        }
    }
}
