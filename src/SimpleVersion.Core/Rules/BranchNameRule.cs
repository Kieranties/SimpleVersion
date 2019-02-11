using System;

namespace SimpleVersion.Rules
{
    public class BranchNameRule : BaseBranchNameRule
    {
        private static Lazy<BranchNameRule> _default = new Lazy<BranchNameRule>(() => new BranchNameRule());

        public static BranchNameRule Instance => _default.Value;

        public BranchNameRule() : base(true)
        {
        }

        public BranchNameRule(string pattern) : base(pattern, true)
        {
        }                

        public override string Token { get; protected set; } = "{branchname}";
    }
}
