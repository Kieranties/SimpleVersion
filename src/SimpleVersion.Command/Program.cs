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

            var (height, version) = repo.GetInfo();

            var formatter = new Semver2Formatter();
            var result = formatter.Format(height, version);

            Console.Out.WriteLine(result.FullVersion);

            return 0;
        }
    }
}
