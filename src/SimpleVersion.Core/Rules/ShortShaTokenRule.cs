// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Abstractions.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Applies rules for the short sha token.
    /// </summary>
    public class ShortShaTokenRule : ITokenRule<string>
    {
        /// <summary>
        /// Gets a default instance of the rule.
        /// </summary>
        public static ShortShaTokenRule Instance => _default.Value;

        private static readonly Lazy<ShortShaTokenRule> _default = new Lazy<ShortShaTokenRule>(() => new ShortShaTokenRule());

        /// <inheritdoc/>
        public string Token => "{shortsha}";

        /// <inheritdoc/>
        public string Resolve(IVersionContext context, string value)
        {
            return Regex.Replace(value, Regex.Escape(Token), $"c{context.Result.Sha7}", RegexOptions.IgnoreCase);
        }

        /// <inheritdoc/>
        public IEnumerable<string> Apply(IVersionContext context, IEnumerable<string> input)
        {
            var isRelease = context.Configuration.Branches.Release
                .Any(x => Regex.IsMatch(context.Result.CanonicalBranchName, x));

            if (!isRelease && !input.Contains(Token))
            {
                return input.Concat(new[] { Token });
            }

            return input;
        }
    }
}
