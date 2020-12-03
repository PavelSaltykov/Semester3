using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Test1
{
    public class Client : IDisposable
    {
        private readonly TcpClient client;

        public Client(string server, int port)
        {
            client = new TcpClient(server, port);
        }

        public void Start()
        {
            var stream = client.GetStream();

            var threadOfSending = new Thread(() =>
            {
                while (true)
                {
                    Send(stream);
                }
            });

            var threadOfReceiving = new Thread(() =>
            {
                while (true)
                {
                    Receive(stream);
                }
            });

            threadOfSending.Start();
            threadOfReceiving.Start();

            threadOfSending.Join();
            threadOfReceiving.Join();
        }

        private void Send(NetworkStream stream)
        {
            var message = Console.ReadLine();
            using var writer = new StreamWriter(stream) { AutoFlush = true };
            writer.WriteLine(message);
        }

        private void Receive(NetworkStream stream)
        {
            using var reader = new StreamReader(stream);
            if (!reader.EndOfStream)
            {
                Console.WriteLine($"Received: { reader.ReadLine() }");
            }
        }

        public void Dispose()
        {
            client.Close();
        }
    }
}
