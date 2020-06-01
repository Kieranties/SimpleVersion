// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Configuration;
using SimpleVersion.Environment;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Encapsulates state for a version request.
    /// </summary>
    public class VersionContext : IVersionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionContext"/> class.
        /// </summary>
        /// <param name="workingDirectory">The working directory of the version request.</param>
        public VersionContext(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
        }

        /// <inheritdoc/>
        public IVersionEnvironment Environment { get; set; } = default!;

        /// <inheritdoc/>
        public VersionConfiguration Configuration { get; set; } = default!;

        /// <inheritdoc/>
        public string WorkingDirectory { get; }

        /// <inheritdoc/>
        public VersionResult Result { get; } = new VersionResult();
    }
}
