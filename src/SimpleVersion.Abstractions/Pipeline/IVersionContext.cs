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
        /// Gets the <see cref="VersionConfiguration"/>.
        /// </summary>
        VersionConfiguration Configuration { get; }

        /// <summary>
        /// Gets the <see cref="IVersionEnvironment"/>.
        /// </summary>
        IVersionEnvironment Environment { get; }

        /// <summary>
        /// Gets the <see cref="VersionResult"/>.
        /// </summary>
        VersionResult Result { get; }
    }
}
