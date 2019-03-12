using SimpleVersion.Pipeline;
using System;

namespace SimpleVersion.Rules
{
    public class ShortBranchNameRule : BaseBranchNameRule
    {
        private static Lazy<ShortBranchNameRule> _default = new Lazy<ShortBranchNameRule>(() => new ShortBranchNameRule());

        public static ShortBranchNameRule Instance => _default.Value;

        public ShortBranchNameRule() : base()
        {
        }

        public ShortBranchNameRule(string pattern) : base(pattern)
        {
        }
        
        public override string Token { get; protected set; } = "{shortbranchname}";

        protected override string ResolveBranchName(VersionContext context) => context.Result.BranchName;
    }
}
