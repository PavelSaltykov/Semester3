using System;
using System.IO;
using System.Linq;

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

                using var rw = new ReportWriter(Console.Out);
                Console.WriteLine("\t\tREPORT:");
                Console.WriteLine();

                var errors = testRunner.GetErrorsInfo();
                if (errors.Count() > 0)
                {
                    Console.WriteLine("  Classes with incorrect methods:");
                    rw.Write(errors);
                }

                testRunner.Run();
                var results = testRunner.GetTestsInfo();
                if (errors.Count() + results.Count() == 0)
                {
                    Console.WriteLine("Test classes not found.");
                    return 0;
                }

                if (results.Count() > 0)
                {
                    Console.WriteLine("  Results:");
                    rw.Write(results);
                }
                return 0;
            }
            catch (Exception e) when
                (e is IOException || e is AssembliesNotFoundException)
            {
                Console.WriteLine($"{e.GetType()}: {e.Message}");
                return 1;
            }
        }
    }
}
