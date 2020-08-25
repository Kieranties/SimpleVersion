// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Text.RegularExpressions;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting the branch name.
    /// </summary>
    public class BranchNameToken : ITokenRequestHandler<BranchNameTokenRequest>
    {
        private static readonly Regex _regex = new Regex("[^a-z0-9]", RegexOptions.Compiled);

        /// <inheritdoc/>
        public string Evaluate(BranchNameTokenRequest request, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(request, nameof(request));
            Assert.ArgumentNotNull(context, nameof(context));

            var branchName = request.BranchName switch
            {
                BranchNameOption.Short => context.Result.BranchName,
                BranchNameOption.Suffix => context.Result.CanonicalBranchName.Substring(context.Result.CanonicalBranchName.LastIndexOf('/') + 1),
                BranchNameOption.Canonical => context.Result.CanonicalBranchName,
                _ => throw new InvalidOperationException($"Invalid option '{request.BranchName}'")
            };

            if (string.IsNullOrWhiteSpace(branchName))
            {
                throw new InvalidOperationException("Branch name has not been set.");
            }

            return _regex.Replace(branchName, string.Empty);
        }
    }

    public enum BranchNameOption
    {
        Short,
        Suffix,
        Canonical
    }

    [Token("branchname", Description = "Provides parsing of the branch name.")]
    public class BranchNameTokenRequest : ITokenRequest
    {
        public BranchNameOption BranchName { get; set; } = BranchNameOption.Canonical;

        public virtual void Parse(string optionValue)
        {
            if (Enum.TryParse<BranchNameOption>(optionValue, out var result))
            {
                this.BranchName = result;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid value");
            }
        }
    }

    [Token("shortbranchname", Description = "Provides parsing of the branch name.")]
    public class ShortBranchNameTokenRequest : BranchNameTokenRequest
    {
        public ShortBranchNameTokenRequest()
        {
            BranchName = BranchNameOption.Short;
        }

        public override void Parse(string optionValue)
        {
            if (!string.IsNullOrWhiteSpace(optionValue))
            {
                throw new InvalidOperationException($"{nameof(ShortBranchNameTokenRequest)} does not support options");
            }
        }
    }
}
