// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Globalization;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of the PR.
    /// </summary>
    public class PrTokenHandler : ITokenHandler
    {
        /// <inheritdoc/>
        public string Key => "pr";

        /// <inheritdoc/>
        public string Process(string? optionValue, IVersionContext context, ITokenEvaluator evaluator)
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
