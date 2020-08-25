// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Globalization;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles token replacement for height.
    /// </summary>
    public class HeightToken : ITokenRequestHandler<HeightTokenRequest>
    {
        /// <inheritdoc/>
        public string Evaluate(HeightTokenRequest request, IVersionContext context, ITokenEvaluator evaluator)
        {
            // assert valid request
            var height = context.Result.Height.ToString(CultureInfo.InvariantCulture);
            return height.PadLeft(request.Padding, '0');
        }
    }

    [Token("*", Description = "Provides parsing of the commit height.")]
    public class HeightTokenRequest : ITokenRequest
    {
        public int Padding { get; set; } = 0;

        public void Parse(string optionValue)
        {
            if(int.TryParse(optionValue, out var result))
            {
                if (result < 0)
                {
                    throw new ArgumentOutOfRangeException("Value must be zero or greater");
                }
                else
                {
                    this.Padding = result;
                }
            }
            else
            {
                throw new ArgumentException("Invalid value");
            }
        }
    }
}
