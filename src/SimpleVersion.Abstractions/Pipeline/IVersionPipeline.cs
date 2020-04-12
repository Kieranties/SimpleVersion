// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Handles version requests.
    /// </summary>
    public interface IVersionPipeline
    {
        /// <summary>
        /// Generates a new <see cref="VersionResult"/>.
        /// </summary>
        /// <returns>A processed <see cref="VersionResult"/>.</returns>
        VersionResult Process();
    }
}
