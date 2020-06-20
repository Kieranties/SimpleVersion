// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting the branch name.
    /// </summary>
    [Token(_tokenKey, DefaultOption = _canonOption, Description = "Provides parsing of the branch name.")]
    [TokenValueOption(_canonOption, Description = "Returns the full canonical branch name.")]
    [TokenValueOption(_suffixOption, Description = "Returns the branch name suffix.")]
    [TokenValueOption(_shortOption, Alias = _shortOption + _tokenKey, Description = "Returns a shortened branch name.")]
    public class BranchNameToken : IToken
    {
        private const string _tokenKey = "branchname";
        private const string _canonOption = "canon";
        private const string _suffixOption = "suffix";
        private const string _shortOption = "short";

        private static readonly Regex _regex = new Regex("[^a-z0-9]", RegexOptions.Compiled);

        /// <inheritdoc/>
        public string Evaluate(string optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(optionValue, nameof(optionValue));
            Assert.ArgumentNotNull(context, nameof(context));

            var branchName = optionValue.ToLowerInvariant() switch
            {
                _shortOption => context.Result.BranchName,
                _suffixOption => context.Result.CanonicalBranchName.Substring(context.Result.CanonicalBranchName.LastIndexOf('/') + 1),
                _canonOption => context.Result.CanonicalBranchName,
                _ => throw new InvalidOperationException($"Invalid option '{optionValue}' for token '{_tokenKey}'")
            };

            if (string.IsNullOrWhiteSpace(branchName))
            {
                throw new InvalidOperationException("Branch name has not been set.");
            }

            return _regex.Replace(branchName, string.Empty);
        }
    }
}
