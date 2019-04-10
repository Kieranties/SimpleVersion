// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;
using System;

namespace SimpleVersion.Rules
{
    public class BranchNameSuffixRule : BaseBranchNameRule
    {
        public static BranchNameSuffixRule Instance => _default.Value;

        private static readonly Lazy<BranchNameSuffixRule> _default = new Lazy<BranchNameSuffixRule>(() => new BranchNameSuffixRule());

        public BranchNameSuffixRule() : base()
        {
        }

        public BranchNameSuffixRule(string pattern) : base(pattern)
        {
        }

        public override string Token { get; protected set; } = "{branchnamesuffix}";

        protected override string ResolveBranchName(VersionContext context)
        {
            return context.Result.CanonicalBranchName.Substring(context.Result.CanonicalBranchName.LastIndexOf('/') + 1);
        }
    }
}
