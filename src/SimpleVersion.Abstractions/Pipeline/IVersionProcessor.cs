// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Handles processing of a <see cref="VersionContext"/>.
    /// </summary>
    public interface IVersionProcessor
    {
        /// <summary>
        /// Process the given <paramref name="context"/> updating
        /// its state if needed.
        /// </summary>
        /// <param name="context">The <see cref="IVersionContext"/> to process.</param>
        void Process(IVersionContext context);
    }
}
