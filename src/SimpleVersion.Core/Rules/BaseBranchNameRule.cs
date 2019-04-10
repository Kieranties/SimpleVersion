// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

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
        private const string _defaultPattern = "[^a-z0-9]";

        protected BaseBranchNameRule() : this(_defaultPattern)
        {
        }

        protected BaseBranchNameRule(string pattern)
        {
            Pattern = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        public abstract string Token { get; protected set; }

        public Regex Pattern { get; protected set; }

        public virtual IEnumerable<string> Apply(VersionContext context, IEnumerable<string> value)
        {
            // No default implementation applies branch name
            return value;
        }

        public virtual string Resolve(VersionContext context, string value)
        {
            if (Regex.IsMatch(value, Token, RegexOptions.IgnoreCase))
            {
                var name = ResolveBranchName(context);
                name = Pattern.Replace(name, "");
                return Regex.Replace(value, Regex.Escape(Token), name, RegexOptions.IgnoreCase);
            }

            return value;
        }

        protected abstract string ResolveBranchName(VersionContext context);
    }
}
