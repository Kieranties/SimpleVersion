// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.IO;

namespace SimpleVersion
{
    /// <summary>
    /// Contract for the calculation process.
    /// Enables collection of processes and invocation to get version results.
    /// </summary>
    public interface IVersionCalculator
    {
        /// <summary>
        /// Gets the processed version result.
        /// </summary>
        /// <returns>A complete <see cref="VersionResult"/> instance.</returns>
        VersionResult GetResult();

        /// <summary>
        /// Writes the processed version result to the <paramref name="output"/>.
        /// </summary>
        /// <param name="output">The writer for the result.</param>
        void WriteResult(TextWriter output);
    }
}
