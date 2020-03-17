// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Applies rules for the friendly branch name token.
    /// </summary>
    public class BranchNameTokenRule : BaseBranchNameTokenRule
    {
        private static readonly Lazy<BranchNameTokenRule> _default = new Lazy<BranchNameTokenRule>(() => new BranchNameTokenRule());

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchNameTokenRule"/> class.
        /// </summary>
        public BranchNameTokenRule() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchNameTokenRule"/> class.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to use when replacing characters.</param>
        public BranchNameTokenRule(string pattern) : base(pattern)
        {
        }

        /// <summary>
        /// Gets a default instance of the rule.
        /// </summary>
        public static BranchNameTokenRule Instance => _default.Value;

        /// <inheritdoc/>
        public override string Token { get; protected set; } = "{branchname}";

        /// <inheritdoc/>
        protected override string ResolveBranchName(IVersionContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            return context.Result.CanonicalBranchName;
        }
    }
}
