// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Rules
{
    public class BranchNameRule : BaseBranchNameRule
    {
        public static BranchNameRule Instance => _default.Value;

        private static readonly Lazy<BranchNameRule> _default = new Lazy<BranchNameRule>(() => new BranchNameRule());

        public BranchNameRule() : base()
        {
        }

        public BranchNameRule(string pattern) : base(pattern)
        {
        }

        public override string Token { get; protected set; } = "{branchname}";

        protected override string ResolveBranchName(VersionContext context) => context.Result.CanonicalBranchName;
    }
}
