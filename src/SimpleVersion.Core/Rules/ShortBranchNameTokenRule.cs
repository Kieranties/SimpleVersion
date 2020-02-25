// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Abstractions.Pipeline;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Applies rules for the short branch name.
    /// </summary>
    public class ShortBranchNameTokenRule : BaseBranchNameTokenRule
    {
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

        /// <summary>
        /// Gets a default instance of the rule.
        /// </summary>
        public static ShortBranchNameTokenRule Instance => _default.Value;

        /// <inheritdoc/>
        public override string Token { get; protected set; } = "{shortbranchname}";

        /// <inheritdoc/>
        protected override string ResolveBranchName(IVersionContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            return context.Result.BranchName;
        }
    }
}
