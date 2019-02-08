using SimpleVersion.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleVersion.Rules
{
    public class ShortShaRule : IRule<string>
    {
        private static Lazy<ShortShaRule> _default = new Lazy<ShortShaRule>(() => new ShortShaRule());

        public static ShortShaRule Instance => _default.Value;

        public string Token => "{shortsha}";

        public string Resolve(VersionContext context, string value)
        {
            var shortSha = context.Result.Sha.Substring(0, 7);
            return Regex.Replace(value, Regex.Escape(Token), $"c{shortSha}", RegexOptions.IgnoreCase);
        }

        public IEnumerable<string> Apply(VersionContext context, IEnumerable<string> input)
        {
            var isRelease = context.Configuration.Branches.Release
                .Any(x => Regex.IsMatch(context.Result.CanonicalBranchName, x));

            if (!isRelease && !input.Contains(Token))
            {
                return input.Concat(new[] { Token });
            }

            return input;
        }
    }
}
