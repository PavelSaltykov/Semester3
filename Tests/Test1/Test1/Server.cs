using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Test1
{
    public class Server
    {
        private readonly IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
        private readonly TcpListener listener;

        public Server(int port)
        {
            listener = new TcpListener(iPAddress, port);
        }

        public void Start()
        {
            try
            {
                listener.Start();
                var socket = listener.AcceptSocket();
                var stream = new NetworkStream(socket);

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
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
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
    }
}
