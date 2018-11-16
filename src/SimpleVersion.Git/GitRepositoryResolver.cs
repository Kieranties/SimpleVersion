using System;
using System.IO;

namespace SimpleVersion.Git
{
    public class GitRepositoryResolver : IRepositoryResolver
    {
        private readonly IVersionModelReader _reader;

        public GitRepositoryResolver(IVersionModelReader reader)
        {
            _reader = reader;
        }

        public IRepository Resolve(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Null or empty repostiory path", nameof(path));
            }

            if (!Directory.Exists(path))
            { 
                throw new DirectoryNotFoundException($"Directory '{path}' does not exist");
            }

            var gitPath = Path.Combine(path, ".git");
            if (!Directory.Exists(gitPath))
            {
                throw new DirectoryNotFoundException($"Could not find git repository '{gitPath}'");
            }

            return new GitRepository(_reader, gitPath);
        }
    }
}
