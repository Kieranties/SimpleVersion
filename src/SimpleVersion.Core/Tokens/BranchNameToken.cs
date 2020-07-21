// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Text.RegularExpressions;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting the branch name.
    /// </summary>
    [TokenValueOption(_canonOption, Description = "Returns the full canonical branch name.")]
    [TokenValueOption(_suffixOption, Description = "Returns the branch name suffix.")]
    [TokenValueOption(_shortOption, Alias = _shortOption + _tokenKey, Description = "Returns a shortened branch name.")]
    public class BranchNameToken : ITokenRequestHandler<BranchNameTokenRequest>
    {
        private const string _tokenKey = "branchname";
        private const string _canonOption = "canon";
        private const string _suffixOption = "suffix";
        private const string _shortOption = "short";

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
                _ => throw new InvalidOperationException($"Invalid option '{request.BranchName}' for token '{_tokenKey}'")
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

        public void Parse(string optionValue)
        {
        }
    }

    [Token("shortbranchname", Description = "Provides parsing of the branch name.")]
    public class ShortBranchNameTokenRequest : BranchNameTokenRequest
    {
        public ShortBranchNameTokenRequest()
        {
            BranchName = BranchNameOption.Short;
        }
    }
}
