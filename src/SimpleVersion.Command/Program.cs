using Newtonsoft.Json;
using SimpleVersion.Git;
using System;

namespace SimpleVersion.Command
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var path = System.IO.Directory.GetCurrentDirectory();
            if (args.Length > 0)
                path = args[0];

            var reader = new JsonVersionInfoReader();
            var repo = new GitRepository(reader, path);

            var result = repo.GetResult();

            Console.Out.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

            return 0;
        }
    }
}
