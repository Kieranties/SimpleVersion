// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Pipeline;

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
}
