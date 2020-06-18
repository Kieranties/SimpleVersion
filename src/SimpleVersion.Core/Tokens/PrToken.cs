// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Globalization;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of the PR.
    /// </summary>
    public class PrToken : BaseToken
    {
        /// <inheritdoc/>
        public override string Key => "pr";

        /// <inheritdoc/>
        public override string Evaluate(IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            if (context.Result.IsPullRequest)
            {
                return context.Result.PullRequestNumber.ToString(CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }
    }
}
