// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Applies rules for the short sha token.
    /// </summary>
    public class ShortShaTokenRule : ITokenRule<string>
    {
        private static readonly Lazy<ShortShaTokenRule> _default = new Lazy<ShortShaTokenRule>(() => new ShortShaTokenRule());

        /// <summary>
        /// Gets a default instance of the rule.
        /// </summary>
        public static ShortShaTokenRule Instance => _default.Value;

        /// <inheritdoc/>
        public string Token => "{shortsha}";

        /// <inheritdoc/>
        public string Resolve(IVersionContext context, string value)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            return Regex.Replace(value, Regex.Escape(Token), $"c{context.Result.Sha7}", RegexOptions.IgnoreCase);
        }

        /// <inheritdoc/>
        public IEnumerable<string> Apply(IVersionContext context, IEnumerable<string> input)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            if (!context.Result.IsRelease && !input.Contains(Token))
            {
                return input.Concat(new[] { Token });
            }

            return input;
        }
    }
}
