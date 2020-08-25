// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

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
            Assert.ArgumentNotNull(request, nameof(request));
            Assert.ArgumentNotNull(context, nameof(context));

            // assert valid request
            var height = context.Result.Height.ToString(CultureInfo.InvariantCulture);
            return height.PadLeft(request.Padding, '0');
        }
    }
}
