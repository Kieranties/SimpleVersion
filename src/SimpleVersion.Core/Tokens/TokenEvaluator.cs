// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles evaluation of a token string.
    /// </summary>
    public class TokenEvaluator : ITokenEvaluator
    {
        private static readonly Regex _regex = new Regex(@"{(?<key>\*|\w+)(?:\:(?<option>(?:[^\}]|\}(?=\}))*))?\}|(?<height>\*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
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
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                return tokenString;
            }

            var charPointer = 0;
            var matches = _regex.Matches(tokenString);

            var result = new StringBuilder();

            // local function to consume unmatched strings
            void ConsumeNonToken(int upToIndex)
            {
                if (charPointer < upToIndex)
                {
                    result.Append(tokenString.Substring(charPointer, upToIndex - charPointer));
                }
            }

            foreach (Match match in matches)
            {
                // Ensure we have consumed up to this token
                ConsumeNonToken(match.Index);

                string? handlerKey = null;
                if (match.Groups["key"].Success)
                {
                    handlerKey = match.Groups["key"].Value;
                }
                else
                {
                    handlerKey = match.Groups["height"].Value;
                }

                if (handlerKey == null)
                {
                    throw new Exception($"Invalid token match for request: {tokenString}");
                }

                var handler = _handlers.FirstOrDefault(t => t.Key == handlerKey);

                if (handler == null)
                {
                    throw new Exception($"Could not find token handler for request: {tokenString}");
                }

                var option = match.Groups["option"].Value;
                result.Append(handler.Process(option, context, this));

                // consume token
                charPointer = match.Index + match.Length;
            }

            // Ensure we consume any final string
            ConsumeNonToken(tokenString.Length);

            return result.ToString();
        }
    }
}
