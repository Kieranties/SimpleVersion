// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Contract for a processor step during version calculation.
    /// </summary>
    public interface IVersionProcessor
    {
        /// <summary>
        /// Applies the processor to the current <see cref="VersionContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="VersionContext"/> of the calculation.</param>
        void Apply(VersionContext context);
    }
}
