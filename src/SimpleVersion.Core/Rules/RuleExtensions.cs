// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;
using System.Collections.Generic;

namespace SimpleVersion.Rules
{
    public static class RuleExtensions
    {
        public static IEnumerable<T> ApplyRules<T>(this IEnumerable<T> value, VersionContext context, IEnumerable<IRule<T>> rules)
        {
            var next = value;
            foreach (var rule in rules)
                next = rule.Apply(context, next);

            return next;
        }

        public static T ResolveRules<T>(this T value, VersionContext context, IEnumerable<IRule<T>> rules)
        {
            var next = value;
            foreach (var rule in rules)
                next = rule.Resolve(context, next);

            return next;
        }
    }
}
