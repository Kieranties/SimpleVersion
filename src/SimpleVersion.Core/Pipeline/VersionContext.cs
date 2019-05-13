using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Model;
using System;
using System.IO;
using Git = LibGit2Sharp;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Versioning context for Git repositories.
    /// </summary>
    public class VersionContext : IVersionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionContext"/> class.
        /// </summary>
        /// <param name="path">The path to the git repository.</param>
        public VersionContext(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path must be provided.", nameof(path));

            var resolvedPath = Git.Repository.Discover(path);

            if (string.IsNullOrWhiteSpace(resolvedPath))
                throw new DirectoryNotFoundException($"Could not find git repository at '{path}' or any parent directory.");

            _repoPath = resolvedPath;
            Result = SetIntialResult();
        }

        /// <inheritdoc/>
        public Configuration Configuration { get; set; } = new Configuration();

        private readonly string _repoPath;

        /// <inheritdoc/>
        public VersionResult Result { get; set; }

        /// <summary>
        /// Gets and instance of the current repository.
        /// </summary>
        /// <returns>The current <see cref="Git.Repository"/> being versioned.</returns>
        public Git.Repository GetRepository() => new Git.Repository(_repoPath);

        private VersionResult SetIntialResult()
        {
            using (var repo = GetRepository())
            {
                return new VersionResult
                {
                    BranchName = repo.Head.FriendlyName,
                    CanonicalBranchName = repo.Head.CanonicalName,
                    Sha = repo.Head.Tip?.Sha
                };
            }
        }
    }
}
