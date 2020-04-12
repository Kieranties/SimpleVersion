// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Configuration;
using SimpleVersion.Pipeline;

namespace SimpleVersion
{
    /// <summary>
    /// Exposes access to repostiory information.
    /// </summary>
    public interface IVersionRepository : IVersionRequestProcessor
    {
        /// <summary>
        /// Gets the <see cref="VersionConfiguration"/> for a specific branch.
        /// If the <paramref name="canonicalBranchName"/> is null, the active
        /// branch in the repository will be used.
        /// </summary>
        /// <param name="canonicalBranchName">The branch name.</param>
        /// <returns>A <see cref="VersionConfiguration"/> for the given branch.</returns>
        VersionConfiguration GetConfiguration(string? canonicalBranchName);
    }
}
