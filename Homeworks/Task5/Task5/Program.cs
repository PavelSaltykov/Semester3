using System;
using System.IO;

namespace Task5
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Incorrect application arguments.");
                return;
            }

            var path = args[0];
            Console.WriteLine($"Path: {path}");
            Console.WriteLine();

            try
            {
                var testRunner = new TestRunner(path);
                testRunner.Run();

                using var rw = new ReportWriter(Console.Out);
                Console.WriteLine("Report:");
                rw.Write(testRunner.GetTestsInfo());
            }
            catch (Exception e) when
                (e is IOException || e is InvalidOperationException || e is AssembliesNotFoundException)
            {
                Console.WriteLine($"{e.GetType()}: {e.Message}");
            }
        }
    }
}
