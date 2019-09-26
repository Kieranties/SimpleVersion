// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Applies rules for the branch name suffix rule.
    /// </summary>
    public class BranchNameSuffixTokenRule : BaseBranchNameTokenRule
    {
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

        /// <summary>
        /// Gets a default instance of the rule.
        /// </summary>
        public static BranchNameSuffixTokenRule Instance => _default.Value;

        /// <inheritdoc/>
        public override string Token { get; protected set; } = "{branchnamesuffix}";

        /// <inheritdoc/>
        protected override string ResolveBranchName(IVersionContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            return context.Result.CanonicalBranchName.Substring(context.Result.CanonicalBranchName.LastIndexOf('/') + 1);
        }
    }
}
