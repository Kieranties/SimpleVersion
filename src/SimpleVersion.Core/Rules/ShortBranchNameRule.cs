using System;

namespace SimpleVersion.Rules
{
    public class ShortBranchNameRule : BaseBranchNameRule
    {
        private static Lazy<ShortBranchNameRule> _default = new Lazy<ShortBranchNameRule>(() => new ShortBranchNameRule());

        public static ShortBranchNameRule Instance => _default.Value;

        public ShortBranchNameRule() : base(false)
        {
        }

        public ShortBranchNameRule(string pattern) : base(pattern, false)
        {
        }
        
        public override string Token { get; protected set; } = "{shortbranchname}";
    }
}
