// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles evaluation of a token string.
    /// </summary>
    public class TokenEvaluator : ITokenEvaluator
    {
        private static readonly Regex _tokenFormat = new Regex(@"\{(?<key>[\w]+)(?:\:(?<option>.+))?\}", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private readonly IEnumerable<ITokenHandler> _handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenEvaluator"/> class.
        /// </summary>
        /// <param name="handlers">The collection of <see cref="ITokenHandler"/> for processing a token string.</param>
        public TokenEvaluator(IEnumerable<ITokenHandler> handlers)
        {
            _handlers = Assert.ArgumentNotNull(handlers, nameof(handlers));
        }

        /// <inheritdoc />
        public string Process(string tokenString, IVersionContext context)
        {
            var (key, option) = ParseTokenString(tokenString);
            var token = _handlers.FirstOrDefault(t => t.Key == key);

            if (token == null)
            {
                throw new Exception($"Could not find token handler for request: {tokenString}");
            }

            return token.Process(option, context, this);
        }

        private (string key, string? option) ParseTokenString(string tokenString)
        {
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                throw new ArgumentOutOfRangeException(nameof(tokenString));
            }

            var match = _tokenFormat.Match(tokenString);
            if (!match.Success)
            {
                throw new Exception($"Invalid token string: {tokenString}");
            }

            return (match.Groups["key"].Value, match.Groups["option"]?.Value);
        }
    }
}
