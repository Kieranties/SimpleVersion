// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using Cake.Core;
using Cake.Core.Annotations;
using SimpleVersion;
using SimpleVersion.Model;

namespace Cake.SimpleVersion
{
    /// <summary>
    /// Aliases exposed to Cake.
    /// </summary>
    public static class CakeAliases
    {
        /// <summary>
        /// Cake method alias to invoke SimpleVersion.
        /// </summary>
        /// <param name="context">The <see cref="ICakeContext"/> of the current build.</param>
        /// <param name="path">The path to the repository to version.</param>
        /// <returns>An <see cref="VersionResult"/> with details of the calculated version.</returns>
        [CakeMethodAlias]
        public static VersionResult SimpleVersion(
            this ICakeContext context,
            string? path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = context.Environment.WorkingDirectory.FullPath;

            return VersionCalculator
                .Default()
                .GetResult(path!); // ! null check is completed above
        }
    }
}
