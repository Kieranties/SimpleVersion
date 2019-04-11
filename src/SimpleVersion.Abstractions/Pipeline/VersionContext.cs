// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Model;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Encapsulates state during version calculation.
    /// </summary>
    public class VersionContext
    {
        /// <summary>
        /// Gets or sets the given path where the version should be calclated.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the actual resolved repository path.
        /// </summary>
        public string RepositoryPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the resolved configuration for the repository.
        /// </summary>
        public Configuration Configuration { get; set; } = new Configuration();

        /// <summary>
        /// Gets or sets the result of the version calculation.
        /// </summary>
        public VersionResult Result { get; set; } = new VersionResult();
    }
}
