// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Text.RegularExpressions;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting the branch name.
    /// </summary>
    public class BranchNameTokenHandler : ITokenHandler
    {
        private static readonly Regex _regex = new Regex("[^a-z0-9]", RegexOptions.Compiled);

        /// <inheritdoc/>
        public virtual string Key => "branchname";

        /// <inheritdoc/>
        public virtual string Process(string? optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            var branchName = optionValue switch
            {
                "short" => context.Result.BranchName,
                "suffix" => context.Result.CanonicalBranchName.Substring(context.Result.CanonicalBranchName.LastIndexOf('/') + 1),
                _ => context.Result.CanonicalBranchName
            };

            return _regex.Replace(branchName, string.Empty);
        }
    }
}
