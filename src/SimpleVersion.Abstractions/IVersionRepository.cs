// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Configuration;

namespace SimpleVersion
{
    /// <summary>
    /// Exposes access to repostiory information.
    /// </summary>
    public interface IVersionRepository
    {
        /// <summary>
        /// Gets the configuration for the repository;
        /// </summary>
        RepositoryConfiguration Configuration { get; }
    }
}
