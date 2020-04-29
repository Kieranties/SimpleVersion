// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;

namespace SimpleVersion.Environment
{
    /// <summary>
    /// Contract for accessing environment variables for a version environment.
    /// </summary>
    public interface IVersionEnvironment
    {
        /// <summary>
        /// Gets the canonical branch name from the environment.
        /// </summary>
        string? CanonicalBranchName { get; }

        /// <summary>
        /// Gets the branch name from the environment.
        /// </summary>
        string? BranchName { get; }

        /// <summary>
        /// Gets a value indicating whether the environment is valid for the current request.
        /// </summary>
        bool IsValid { get; }
    }
}
