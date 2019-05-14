// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Abstractions.Rules;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Applies rules for the pull-request token.
    /// </summary>
    public class PullRequestIdTokenRule : ITokenRule<string>
    {
        /// <summary>
        /// Gets a default instance of the rule.
        /// </summary>
        public static PullRequestIdTokenRule Instance => _default.Value;

        private static readonly Lazy<PullRequestIdTokenRule> _default = new Lazy<PullRequestIdTokenRule>(() => new PullRequestIdTokenRule());

        /// <inheritdoc/>
        public string Token => "{pr}";

        /// <inheritdoc/>
        public string Resolve(IVersionContext context, string value)
        {
            return Regex.Replace(value, Regex.Escape(Token), context.Result.PullRequestNumber.ToString(System.Globalization.CultureInfo.CurrentCulture), RegexOptions.IgnoreCase);
        }

        /// <inheritdoc/>
        public IEnumerable<string> Apply(IVersionContext context, IEnumerable<string> input)
        {
            return input;
        }
    }
}
