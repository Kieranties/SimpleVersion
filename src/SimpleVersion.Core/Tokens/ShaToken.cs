// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;
using System;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Exposes the git sha as a token for consumption.
    /// </summary>
    public class ShaToken : ITokenRequestHandler<ShaTokenRequest>
    {
        /// <inheritdoc/>
        public string Evaluate(ShaTokenRequest request, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(request, nameof(request));
            Assert.ArgumentNotNull(context, nameof(context));

            var result = context.Result.Sha;

            return "c" + result.Substring(0, request.Length);
        }
    }

    [Token("sha", Description = "Provides parsing of the commit sha.")]
    public class ShaTokenRequest : ITokenRequest
    {
        // TODO: handle 'short', 'full', greater than 0;
        public int Length { get; set; } = 40;

        public void Parse(string optionValue)
        {
            if(int.TryParse(optionValue, out var result))
            {
                if(result < 1 || result > 40)
                {
                    throw new ArgumentOutOfRangeException("Must be between 1 and 40");
                }

                Length = result;
            }

            throw new ArgumentException("Invalid value");
        }
    }

    [Token("shortsha", Description = "Provides parsing of the commit sha.")]
    public class ShortShaTokenRequest : ShaTokenRequest
    {
        public ShortShaTokenRequest()
        {
            Length = 7;
        }
    }
}
