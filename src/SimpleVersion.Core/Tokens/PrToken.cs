// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Globalization;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of the PR.
    /// </summary>
    public class PrToken : ITokenRequestHandler<PrTokenRequest>
    {
        /// <inheritdoc/>
        public string Evaluate(PrTokenRequest request, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            if (context.Result.IsPullRequest)
            {
                return context.Result.PullRequestNumber.ToString(CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }
    }

    [Token("pr", Description = "Provides parsing of the pull-request number.")]
    public class PrTokenRequest : ITokenRequest
    {
        public void Parse(string optionValue)
        {
            if(!string.IsNullOrWhiteSpace(optionValue))
            {
                throw new InvalidOperationException("Option values not supported.");
            }
        }
    }
}
