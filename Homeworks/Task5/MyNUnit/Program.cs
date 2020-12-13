using System;
using System.IO;

namespace MyNUnit
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WindowWidth = Console.LargestWindowWidth - 10;
            if (args.Length != 1)
            {
                Console.WriteLine("Please enter the path to assemblies.");
                return 1;
            }

            var path = args[0];
            Console.WriteLine($"Path: {path}");
            Console.WriteLine();

            try
            {
                var testRunner = new TestRunner(path);
                testRunner.Run();

                using var rw = new ReportWriter(Console.Out);
                Console.WriteLine("\tREPORT:");
                Console.WriteLine();
                rw.Write(testRunner.GetTestsInfo());
                return 0;
            }
            catch (Exception e) when
                (e is IOException || e is InvalidOperationException || e is AssembliesNotFoundException)
            {
                Console.WriteLine($"{e.GetType()}: {e.Message}");
                return 1;
            }
        }
    }
}
