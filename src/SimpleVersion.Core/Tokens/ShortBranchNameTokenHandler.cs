// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Legacy handler for short branch name.
    /// </summary>
    public class ShortBranchNameTokenHandler : BranchNameTokenHandler
    {
        /// <inheritdoc/>
        public override string Key => "shortbranchname";

        /// <inheritdoc/>
        public override string Process(string? optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            return evaluator.Process("{branchname:short}", context);
        }
    }
}