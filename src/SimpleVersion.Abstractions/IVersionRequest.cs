// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Environment;

namespace SimpleVersion
{
    /// <summary>
    /// Contract for a version request.
    /// </summary>
    public interface IVersionRequest
    {
        /// <summary>
        /// Gets the repostitory for this request.
        /// </summary>
        IVersionRepository Repository { get; }

        /// <summary>
        /// Gets the environment for this request.
        /// </summary>
        IVersionEnvironment Environment { get; }
    }
}
