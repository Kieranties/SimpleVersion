// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Handles processing of a <see cref="VersionContext"/>.
    /// </summary>
    public interface IVersionRequestProcessor
    {
        /// <summary>
        /// Processess the given <paramref name="context"/> updating
        /// its state if needed.
        /// </summary>
        /// <param name="context">The <see cref="VersionContext"/> to process.</param>
        void Process(VersionContext context);
    }
}
