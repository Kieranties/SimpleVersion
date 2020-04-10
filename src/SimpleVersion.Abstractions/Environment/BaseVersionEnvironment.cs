// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Environment
{
    /// <summary>
    /// Base implementation to read environment variables.
    /// </summary>
    public abstract class BaseVersionEnvironment : IVersionEnvironment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseVersionEnvironment"/> class.
        /// </summary>
        /// <param name="accessor">The accessor for environment variables.</param>
        public BaseVersionEnvironment(IEnvironmentVariableAccessor accessor)
        {
            Variables = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        /// <inheritdoc/>
        public abstract string? CanonicalBranchName { get; }

        /// <inheritdoc/>
        public abstract string? BranchName { get; }

        /// <inheritdoc/>
        public abstract bool IsValid { get; }

        /// <inheritdoc/>
        public IEnvironmentVariableAccessor Variables { get; }
    }
}
