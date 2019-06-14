// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Model;
using System;
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
        /// <param name="repository">The git repository.</param>
        public VersionContext(Git.IRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Result = SetIntialResult();
        }

        /// <inheritdoc/>
        public Configuration Configuration { get; set; } = new Configuration();

        /// <inheritdoc/>
        public VersionResult Result { get; set; }

        /// <summary>
        /// Gets and instance of the current repository.
        /// </summary>
        /// <returns>The current <see cref="Git.Repository"/> being versioned.</returns>
        public Git.IRepository Repository { get; }

        private VersionResult SetIntialResult()
        {
            var sha = Repository.Head.Tip?.Sha;
            return new VersionResult
            {
                BranchName = Repository.Head.FriendlyName,
                CanonicalBranchName = Repository.Head.CanonicalName,
                Sha = sha,
                Sha7 = (string.IsNullOrEmpty(sha) || sha.Length < 8) ? null : sha.Substring(0, 7)
        };
        }
    }
}
