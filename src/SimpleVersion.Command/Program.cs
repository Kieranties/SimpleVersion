using Newtonsoft.Json;
using SimpleVersion.Pipeline;
using System;

namespace SimpleVersion.Command
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var exitCode = 0;
            try {
                var path = System.IO.Directory.GetCurrentDirectory();
                if (args.Length > 0)
                    path = args[0];

                var result = VersionCalculator
                    .Default()
                    .GetResult(path);

                Console.Out.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));                
            }
            catch (Exception ex) {
                Console.Error.WriteLine($"[Error] {ex.Message}");
                exitCode = -1;
            }

            return exitCode;
        }
    }
}
