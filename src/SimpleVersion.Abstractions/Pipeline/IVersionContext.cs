// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Configuration;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Encapsulates state during version calculation.
    /// </summary>
    public interface IVersionContext
    {
        /// <summary>
        /// Gets or sets the resolved configuration for the repository.
        /// </summary>
        RepositoryConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the result of the version calculation.
        /// </summary>
        VersionResult Result { get; set; }
    }
}
