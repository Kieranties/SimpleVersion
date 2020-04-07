// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Configuration;
using SimpleVersion.Exceptions;
using Git = LibGit2Sharp;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Version context for Git repositories.
    /// </summary>
    public class VersionContext : IVersionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionContext"/> class.
        /// </summary>
        /// <param name="repository">The git repository.</param>
        public VersionContext(Git.IRepository repository)
        {
            Repository = Assert.ArgumentNotNull(repository, nameof(repository));
            Result = SetInitialResult();
        }

        /// <inheritdoc/>
        public RepositoryConfiguration Configuration { get; set; } = new RepositoryConfiguration();

        /// <inheritdoc/>
        public VersionResult Result { get; set; }

        /// <summary>
        /// Gets and instance of the current repository.
        /// </summary>
        /// <returns>The current <see cref="Git.Repository"/> being versioned.</returns>
        public Git.IRepository Repository { get; }

        private VersionResult SetInitialResult()
        {
            var sha = Repository.Head.Tip?.Sha;
            if (sha == null)
            {
                throw new GitException(Resources.Exception_CouldNotFindBranchTip);
            }

            return new VersionResult
            {
                BranchName = Repository.Head.FriendlyName,
                CanonicalBranchName = Repository.Head.CanonicalName,
                Sha = sha,
                Sha7 = sha.Length > 7 ? sha.Substring(0, 7) : sha
            };
        }
    }
}
