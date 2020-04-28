// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;

namespace SimpleVersion
{
    /// <summary>
    /// Exposes access to repostiory information.
    /// </summary>
    public interface IVersionRepository
    {
        /// <summary>
        /// Gets the <see cref="VersionResult"/> for the repository.
        /// </summary>
        /// <returns>A populated <see cref="VersionResult"/>.</returns>
        VersionResult GetResult();
    }
}
