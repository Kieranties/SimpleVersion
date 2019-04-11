// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;
using System;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Applies rules for the branch name suffix rule.
    /// </summary>
    public class BranchNameSuffixTokenRule : BaseBranchNameTokenRule
    {
        /// <summary>
        /// Gets a default instance of the rule.
        /// </summary>
        public static BranchNameSuffixTokenRule Instance => _default.Value;

        private static readonly Lazy<BranchNameSuffixTokenRule> _default = new Lazy<BranchNameSuffixTokenRule>(() => new BranchNameSuffixTokenRule());

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchNameSuffixTokenRule"/> class.
        /// </summary>
        public BranchNameSuffixTokenRule() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchNameSuffixTokenRule"/> class.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to use when replacing characters.</param>
        public BranchNameSuffixTokenRule(string pattern) : base(pattern)
        {
        }

        /// <inheritdoc/>
        public override string Token { get; protected set; } = "{branchnamesuffix}";

        /// <inheritdoc/>
        protected override string ResolveBranchName(VersionContext context)
        {
            return context.Result.CanonicalBranchName.Substring(context.Result.CanonicalBranchName.LastIndexOf('/') + 1);
        }
    }
}
