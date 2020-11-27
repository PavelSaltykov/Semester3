﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FtpClient
{
    public class Client : IDisposable
    {
        private readonly TcpClient client;
        private readonly Stream stream;
        private readonly StreamWriter writer;
        private readonly StreamReader reader;

        public Client(string hostname, int port)
        {
            client = new TcpClient(hostname, port);
            stream = client.GetStream();
            writer = new StreamWriter(stream) { AutoFlush = true };
            reader = new StreamReader(stream);

        }

        public async Task<IEnumerable<(string name, bool isDir)>> ListAsync(string path)
        {
            await writer.WriteLineAsync($"1 {path}");
            var response = await reader.ReadLineAsync();
            if (response == "-1")
                throw new InvalidOperationException($"Directory not found at path: {path}.");

            var splittedResponse = response.Split(' ');
            var size = int.Parse(splittedResponse[0]);

            if (splittedResponse.Length != 1 + size * 2)
                throw new InvalidOperationException("Incorrect response.");

            var result = new List<(string, bool)>();
            for (var i = 0; i < size; ++i)
            {
                result.Add((splittedResponse[i * 2 + 1], bool.Parse(splittedResponse[i * 2 + 2])));
            }
            return result;
        }

        public async Task GetAsync(string path, string destinationPath, string filename)
        {
            await writer.WriteLineAsync($"2 {path}");
            if (reader.Peek() == '-')
            {
                await reader.ReadLineAsync();
                throw new InvalidOperationException($"File not found at path: {path}");
            }

            var size = new char[long.MaxValue.ToString().Length + 1];
            var index = -1;
            do
            {
                index++;
                await reader.ReadAsync(size, index, 1);

            } while (size[index] != ' ');

            await Download(long.Parse(size), destinationPath, filename);
        }

        private async Task Download(long size, string destinationPath, string filename)
        {
            if (Path.GetFileName(filename) == string.Empty)
                throw new InvalidOperationException($"Incorrect filename: {filename}.");

            string directoryPath = $@"{Directory.GetCurrentDirectory()}\{destinationPath}";
            Directory.CreateDirectory(directoryPath);
            using var fileStream = File.Create(@$"{directoryPath}\{filename}");

            const int maxBufferSize = 81920;
            while (size > 0)
            {
                var buffer = new byte[maxBufferSize];
                var currentBufferSize = size > maxBufferSize ? maxBufferSize : (int)size;
                await stream.ReadAsync(buffer, 0, currentBufferSize);
                await fileStream.WriteAsync(buffer, 0, currentBufferSize);
                size -= maxBufferSize;
            }
        }

        public void Dispose()
        {
            client.Dispose();
            stream.Dispose();
            writer.Dispose();
            reader.Dispose();
        }
    }
}
