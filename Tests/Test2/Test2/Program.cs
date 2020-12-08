using System;
using System.Diagnostics;

namespace Test2
{
    public class Program
    {
        private static string ByteArrayToString(byte[] ba) => BitConverter.ToString(ba).Replace("-", "");

        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Incorrect application arguments.");
                return;
            }

            var path = args[0];
            Console.WriteLine($"Path: {path}");
            Console.WriteLine("Check-sum: ");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var singleThreadedRes = CheckSum.ComputeSingleThreaded(path);
            stopwatch.Stop();

            Console.WriteLine($"Single-threaded result: {ByteArrayToString(singleThreadedRes)}");
            Console.WriteLine($"Time: {stopwatch.Elapsed}");
            Console.WriteLine();

            stopwatch = Stopwatch.StartNew();
            var multiThreadedRes = CheckSum.ComputeMultiThreaded(path).Result;
            stopwatch.Stop();

            Console.WriteLine($"Multi-threaded result: {ByteArrayToString(multiThreadedRes)}");
            Console.WriteLine($"Time: {stopwatch.Elapsed}");
        }
    }
}
