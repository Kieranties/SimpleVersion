using LibGit2Sharp;
using System.IO;

namespace SimpleVersion.Pipeline
{
    public class ResolveRepositoryPathProcess : ICalculatorProcess
    {
        public void Apply(VersionContext context)
        {
            var resolvedPath = Repository.Discover(context.Path);

            if (string.IsNullOrWhiteSpace(resolvedPath))
            {
                throw new DirectoryNotFoundException($"Could not find git repository at '{context.Path}' or any parent directory");
            }

            context.Path = resolvedPath;
        }
    }
}
