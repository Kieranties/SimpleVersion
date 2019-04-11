// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;
using System;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Applies rules for the short branch name.
    /// </summary>
    public class ShortBranchNameTokenRule : BaseBranchNameTokenRule
    {
        /// <summary>
        /// Gets a default instance of the rule.
        /// </summary>
        public static ShortBranchNameTokenRule Instance => _default.Value;

        private static readonly Lazy<ShortBranchNameTokenRule> _default = new Lazy<ShortBranchNameTokenRule>(() => new ShortBranchNameTokenRule());

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortBranchNameTokenRule"/> class.
        /// </summary>
        public ShortBranchNameTokenRule() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortBranchNameTokenRule"/> class.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to use when replacing characters.</param>
        public ShortBranchNameTokenRule(string pattern) : base(pattern)
        {
        }

        /// <inheritdoc/>
        public override string Token { get; protected set; } = "{shortbranchname}";

        /// <inheritdoc/>
        protected override string ResolveBranchName(VersionContext context) => context.Result.BranchName;
    }
}
