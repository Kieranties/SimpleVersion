// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    // TODO: Handle circular token parsing (e.g. {label} => "a", "{label}", "b")

    /// <summary>
    /// Handles evaluation of a token string.
    /// </summary>
    public class TokenEvaluator : ITokenEvaluator
    {
        private static readonly Regex _regex = new Regex(@"{(?<key>\*|\w+)(?:\:(?<option>(?:[^\}]|\}(?=\}))*))?\}|(?<height>\*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private readonly Dictionary<string, IToken> _tokenCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenEvaluator"/> class.
        /// </summary>
        /// <param name="tokens">The collection of <see cref="IToken"/> for processing a token string.</param>
        public TokenEvaluator(IEnumerable<IToken> tokens)
        {
            _tokenCache = Assert.ArgumentNotNull(tokens, nameof(tokens))
                .ToDictionary(t => t.Key, StringComparer.OrdinalIgnoreCase);
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
                    result.Append(tokenString[charPointer..upToIndex]);
                }
            }

            foreach (Match match in matches)
            {
                // Ensure we have consumed up to this token
                ConsumeNonToken(match.Index);

                string? tokenKey = null;
                if (match.Groups["key"].Success)
                {
                    tokenKey = match.Groups["key"].Value;
                }
                else
                {
                    tokenKey = match.Groups["height"].Value;
                }

                if (tokenKey == null)
                {
                    throw new Exception($"Invalid token match for request: {tokenString}");
                }

                if (!_tokenCache.TryGetValue(tokenKey, out var token))
                {
                    throw new Exception($"Could not find token handler for request: {tokenString}");
                }

                var optionGroup = match.Groups["option"];
                var tokenResult = optionGroup.Success
                    ? token.EvaluateWithOption(optionGroup.Value, context, this)
                    : token.Evaluate(context, this);

                result.Append(tokenResult);

                // consume token
                charPointer = match.Index + match.Length;
            }

            // Ensure we consume any final string
            ConsumeNonToken(tokenString.Length);

            return result.ToString();
        }

        /// <inheritdoc />
        public string Process<TToken>(IVersionContext context)
            where TToken : IToken
        {
            var token = _tokenCache.Values.OfType<TToken>().Single();

            return Process($"{{{token.Key}}}", context);
        }

        /// <inheritdoc />
        public string Process<TToken>(string optionValue, IVersionContext context)
            where TToken : IToken
        {
            var token = _tokenCache.Values.OfType<TToken>().Single();

            return Process($"{{{token.Key}:{optionValue}}}", context);
        }
    }
}
