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
        private readonly IServiceProvider _services;
        private readonly Dictionary<string, Type> _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenEvaluator"/> class.
        /// </summary>
        /// <param name="tokens">The collection of <see cref="IToken"/> for processing a token string.</param>
        public TokenEvaluator(IServiceProvider services)
        {
            _services = services;

            var types = this.GetType()
                .Assembly
                .GetTypes()
                .Where(t => t.FindInterfaces((c, _) => c == typeof(ITokenRequest), null).Any());

            // TODO: built smarter cache/factory
            _cache = types
                .ToDictionary(
                    t =>
                    {
                        var attrs = t.GetCustomAttributes(typeof(TokenAttribute), false).OfType<TokenAttribute>();
                        return attrs.Single().Key;

                    },
                    StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public string Parse(string tokenString, IVersionContext context)
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

                if (!_cache.TryGetValue(tokenKey, out var requestType))
                {
                    throw new Exception($"Could not find token handler for request: {tokenString}");
                }

                var token = Activator.CreateInstance(requestType) as ITokenRequest;
                var optionGroup = match.Groups["option"];
                token.Parse(optionGroup.Value);

                var tokenResult = Process(token, context);

                result.Append(tokenResult);

                // consume token
                charPointer = match.Index + match.Length;
            }

            // Ensure we consume any final string
            ConsumeNonToken(tokenString.Length);

            return result.ToString();
        }

        /// <inheritdoc />
        public string Process(ITokenRequest request, IVersionContext context)
        {
            // TODO: less mess
            var handlerType = typeof(ITokenRequestHandler<>).MakeGenericType(request.GetType());
            var handler = _services.GetService(handlerType);
            return (string)handlerType
                .GetMethod("Evaluate")
                .Invoke(handler, new object[] { request, context, this });
        }
    }
}
