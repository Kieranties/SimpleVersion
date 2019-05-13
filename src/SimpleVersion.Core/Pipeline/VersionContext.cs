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
    public sealed class VersionContext : IVersionContext, IDisposable
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

            Repository = new Git.Repository(resolvedPath);
            SetIntialResult();
        }

        /// <inheritdoc/>
        public Configuration Configuration { get; set; } = new Configuration();

        /// <inheritdoc/>
        public VersionResult Result { get; set; } = new VersionResult();

        /// <summary>
        /// Gets the git repository for this context.
        /// </summary>
        public Git.Repository Repository { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (Repository != null)
            {
                Repository.Dispose();
            }
        }

        private VersionResult SetIntialResult()
        {
            return new VersionResult
            {
                BranchName = Repository.Head.FriendlyName,
                CanonicalBranchName = Repository.Head.CanonicalName,
                Sha = Repository.Head.Tip.Sha
            };
        }
    }
}
