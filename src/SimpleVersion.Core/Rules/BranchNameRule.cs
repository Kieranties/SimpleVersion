using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Rules
{
    public class BranchNameRule : IRule<string>
    {
        private static Lazy<BranchNameRule> _default = new Lazy<BranchNameRule>(() => new BranchNameRule());
        private static string _defaultPattern = "[^a-z0-9]";

        public static BranchNameRule Instance => _default.Value;

        public string Token => "{branchname}";

        public BranchNameRule(): this(_defaultPattern, false)
        {

        }

        public BranchNameRule(bool useCanonical) : this(_defaultPattern, useCanonical)
        {
        }

        public BranchNameRule(string pattern, bool useCanonical)
        {
            Pattern = new Regex(pattern, RegexOptions.IgnoreCase);
            UseCanonical = useCanonical;
        }

        public Regex Pattern { get; }

        public bool UseCanonical { get; }

        public IEnumerable<string> Apply(VersionContext context, IEnumerable<string> value)
        {
            // No default implementation applies branch name
            return value;
        }

        public string Resolve(VersionContext context, string value)
        {
            var name = UseCanonical ? context.Result.CanonicalBranchName : context.Result.BranchName;
            name = Pattern.Replace(name, "");
            return Regex.Replace(value, Regex.Escape(Token), name, RegexOptions.IgnoreCase);
        }
    }
}
