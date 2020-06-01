// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Configuration;
using SimpleVersion.Environment;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Encapsulates state during version calculation.
    /// </summary>
    public interface IVersionContext
    {
        /// <summary>
        /// Gets or sets the <see cref="VersionConfiguration"/>.
        /// </summary>
        VersionConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IVersionEnvironment"/>.
        /// </summary>
        IVersionEnvironment Environment { get; set; }

        /// <summary>
        /// Gets the working directory.
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        /// Gets the <see cref="VersionResult"/>.
        /// </summary>
        VersionResult Result { get; }
    }
}
