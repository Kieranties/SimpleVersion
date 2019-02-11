using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Rules
{
    public abstract class BaseBranchNameRule : IRule<string>
    {
        protected static string _defaultPattern = "[^a-z0-9]";


        protected BaseBranchNameRule(bool useCanonical) : this(_defaultPattern, useCanonical)
        {
        }

        protected BaseBranchNameRule(string pattern, bool useCanonical)
        {
            Pattern = new Regex(pattern, RegexOptions.IgnoreCase);
            UseCanonical = useCanonical;
        }

        public abstract string Token { get; protected set; }

        public Regex Pattern { get; protected set; }

        public bool UseCanonical { get; protected set; }

        public virtual IEnumerable<string> Apply(VersionContext context, IEnumerable<string> value)
        {
            // No default implementation applies branch name
            return value;
        }

        public virtual string Resolve(VersionContext context, string value)
        {
            var name = ResolveBranchName(context);
            name = Pattern.Replace(name, "");
            return Regex.Replace(value, Regex.Escape(Token), name, RegexOptions.IgnoreCase);
        }

        protected virtual string ResolveBranchName(VersionContext context)
        {
            return UseCanonical ? context.Result.CanonicalBranchName : context.Result.BranchName;
        }
    }
}
