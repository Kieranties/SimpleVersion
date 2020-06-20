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
    public class BranchNameToken : BaseToken
    {
        public static class Options
        {
            public const string Short = "short";
            public const string Suffix = "suffix";
            public const string Canon = "canon";
            public const string Default = Canon;
        }

        private static readonly Regex _regex = new Regex("[^a-z0-9]", RegexOptions.Compiled);

        /// <inheritdoc/>
        public override string Key => "branchname";

        /// <inheritdoc/>
        public override bool SupportsOptions => true;

        /// <inheritdoc/>
        public override string Evaluate(IVersionContext context, ITokenEvaluator evaluator)
        {
            return EvaluateWithOption(Options.Default, context, evaluator);
        }

        /// <inheritdoc/>
        protected override string EvaluateWithOptionImpl(string optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(optionValue, nameof(optionValue));
            Assert.ArgumentNotNull(context, nameof(context));

            var branchName = optionValue.ToLowerInvariant() switch
            {
                Options.Short => context.Result.BranchName,
                Options.Suffix => context.Result.CanonicalBranchName.Substring(context.Result.CanonicalBranchName.LastIndexOf('/') + 1),
                Options.Default => context.Result.CanonicalBranchName,
                _ => throw new InvalidOperationException($"Invalid option '{optionValue}' for token '{Key}'")
            };

            if (string.IsNullOrWhiteSpace(branchName))
            {
                throw new InvalidOperationException("Branch name has not been set.");
            }

            return _regex.Replace(branchName, string.Empty);
        }
    }
}
