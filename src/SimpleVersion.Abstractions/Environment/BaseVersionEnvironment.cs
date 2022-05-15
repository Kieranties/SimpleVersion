// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Text.RegularExpressions;

namespace SimpleVersion.Environment
{
    /// <summary>
    /// Base implementation to read environment variables.
    /// </summary>
    public abstract class BaseVersionEnvironment : IVersionEnvironment
    {
        /// <summary>
        /// Use to normalize branch names from a canonical name.
        /// </summary>
        protected static readonly Regex CanonicalBranchTrimmer = new Regex(@"^refs\/(heads\/)?", RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseVersionEnvironment"/> class.
        /// </summary>
        /// <param name="accessor">The accessor for environment variables.</param>
        protected BaseVersionEnvironment(IEnvironmentVariableAccessor accessor)
        {
            Variables = accessor ?? throw new ArgumentNullException(nameof(accessor));
            CanonicalBranchName = Variables.GetVariable("simpleversion.sourcebranch");
            if (CanonicalBranchName != null)
            {
                BranchName = CanonicalBranchTrimmer.Replace(CanonicalBranchName, string.Empty);
            }
        }

        /// <inheritdoc/>
        public string? CanonicalBranchName { get; protected set; }

        /// <inheritdoc/>
        public string? BranchName { get; protected set; }

        /// <inheritdoc/>
        public abstract bool IsValid { get; }

        /// <summary>
        /// Gets the <see cref="IEnvironmentVariableAccessor"/> to access
        /// environment variables.
        /// </summary>
        protected IEnvironmentVariableAccessor Variables { get; }
    }
}
