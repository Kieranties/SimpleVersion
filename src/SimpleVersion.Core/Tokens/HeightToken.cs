// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Globalization;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles token replacement for height.
    /// </summary>
    [Token(_tokenKey, DefaultOption = _noPaddingOption, Description = "Provides parsing of the commit height.")]
    [TokenValueOption(_noPaddingOption, Description = "Performs no padding when returning the height.")]
    [TokenFallbackOption("Provide a number greater than 0 to pad the returned height to that many digits.")]
    public class HeightToken : IToken
    {
        private const string _tokenKey = "*";
        private const string _noPaddingOption = "0";

        /// <inheritdoc/>
        public string Evaluate(string optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            if (int.TryParse(optionValue, out var padding))
            {
                var height = context.Result.Height.ToString(CultureInfo.InvariantCulture);
                return height.PadLeft(padding, '0');
            }

            throw new InvalidOperationException($"Invalid option for height token: '{optionValue}'");
        }
    }
}
