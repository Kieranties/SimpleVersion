// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Model;

namespace SimpleVersion.Abstractions
{
    /// <summary>
    /// Contract for the calculation process.
    /// Enables collection of processes and invocation to get version results.
    /// </summary>
    public interface IVersionCalculator
    {
        /// <summary>
        /// Invokes the chain of processors to get a <see cref="VersionResult"/>.
        /// </summary>
        /// <param name="path">The path to the repository to version.</param>
        /// <returns>The resulting <see cref="VersionResult"/>.</returns>
        VersionResult GetResult(string path);
    }
}
