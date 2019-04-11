// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Base implementation to apply rules for branch name tokens.
    /// </summary>
    public abstract class BaseBranchNameTokenRule : ITokenRule<string>
    {
        private const string _defaultPattern = "[^a-z0-9]";

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseBranchNameTokenRule"/> class.
        /// </summary>
        protected BaseBranchNameTokenRule() : this(_defaultPattern)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseBranchNameTokenRule"/> class
        /// with an optional regular expression pattern.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to use when replacing characters.</param>
        protected BaseBranchNameTokenRule(string pattern)
        {
            Pattern = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        /// <inheritdoc/>
        public abstract string Token { get; protected set; }

        /// <summary>
        /// Gets or sets the resolved pattern used to replace non-required characters.
        /// </summary>
        public Regex Pattern { get; protected set; }

        /// <inheritdoc/>
        public virtual IEnumerable<string> Apply(VersionContext context, IEnumerable<string> value)
        {
            // No default implementation applies branch name
            return value;
        }

        /// <inheritdoc/>
        public virtual string Resolve(VersionContext context, string value)
        {
            if (Regex.IsMatch(value, Token, RegexOptions.IgnoreCase))
            {
                var name = ResolveBranchName(context);
                name = Pattern.Replace(name, string.Empty);
                return Regex.Replace(value, Regex.Escape(Token), name, RegexOptions.IgnoreCase);
            }

            return value;
        }

        /// <summary>
        /// Resolves the branch name value for this rule.
        /// </summary>
        /// <param name="context">The <see cref="VersionContext"/> of the current calculation.</param>
        /// <returns>The branch name value.</returns>
        protected abstract string ResolveBranchName(VersionContext context);
    }
}
