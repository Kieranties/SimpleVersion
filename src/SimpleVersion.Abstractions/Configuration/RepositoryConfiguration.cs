// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Configuration
{
    /// <summary>
    /// Encapsulates configuration loaded from '.simpleversion.json'.
    /// </summary>
    public class RepositoryConfiguration : VersionConfiguration
    {
        /// <summary>
        /// Gets or sets the information on branches.
        /// See <see cref="BranchConfiguration"/> for further details.
        /// </summary>
        public BranchConfiguration Branches { get; set; } = new BranchConfiguration();
    }
}
