// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Exposes the git sha as a token for consumption.
    /// </summary>
    [Token(_tokenKey, DefaultOption = _fullOption, Description = "Provides parsing of the commit sha.")]
    [TokenValueOption(_fullOption, Description = "Returns the full commit sha.")]
    [TokenValueOption(_shortOption, Alias = _shortOption + _tokenKey, Description = "Returns the first seven characters of the commit sha.")]
    [TokenFallbackOption("Provide a number greater than 0 to return up to that many characters from the commit sha.")]
    public class ShaToken : IToken
    {
        private const string _tokenKey = "sha";
        private const string _fullOption = "full";
        internal const string _shortOption = "short";

        /// <inheritdoc/>
        public string Evaluate(string optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(optionValue, nameof(optionValue));
            Assert.ArgumentNotNull(context, nameof(context));

            var result = context.Result.Sha;

            int ParseOptionAsInt()
            {
                if (int.TryParse(optionValue, out var length) && length > 0)
                {
                    return Math.Min(length, result.Length);
                }

                throw new InvalidOperationException($"Invalid sha option '{optionValue}'.  Expected an integer greater than 0, 'full', or 'short'.");
            }

            var length = optionValue.ToLowerInvariant() switch
            {
                _fullOption => result.Length,
                _shortOption => 7,
                _ => ParseOptionAsInt()
            };

            return "c" + result.Substring(0, length);
        }
    }
}
