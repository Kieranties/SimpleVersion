// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;
using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Abstractions.Rules;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Rules
{
    /// <summary>
    /// Extension methods to handle rules.
    /// </summary>
    public static class TokenRuleExtensions
    {
        /// <summary>
        /// Applies an enumerable of token rules to a value.
        /// </summary>
        /// <typeparam name="T">The type of value to be processed.</typeparam>
        /// <param name="value">The instance value to be processed.</param>
        /// <param name="context">The <see cref="VersionContext"/> of the current version calculation.</param>
        /// <param name="rules">The enumerable of rules to apply.</param>
        /// <returns>The value once all rules have been applied.</returns>
        public static IEnumerable<T> ApplyTokenRules<T>(this IEnumerable<T> value, IVersionContext context, IEnumerable<ITokenRule<T>> rules)
        {
            if (rules == null)
            {
                return value;
            }

            var next = value;
            foreach (var rule in rules)
            {
                next = rule.Apply(context, next);
            }

            return next;
        }

        /// <summary>
        /// Resolves an enumerable of token rules against a value.
        /// </summary>
        /// <typeparam name="T">The type of value to be processed.</typeparam>
        /// <param name="value">The instance value to be processed.</param>
        /// <param name="context">The <see cref="VersionContext"/> of the current version calculation.</param>
        /// <param name="rules">The enumerable of rules to resolve.</param>
        /// <returns>The value once all rules have been resolved.</returns>
        public static T ResolveTokenRules<T>(this T value, IVersionContext context, IEnumerable<ITokenRule<T>> rules)
        {
            if (rules == null)
            {
                return value;
            }

            var next = value;
            foreach (var rule in rules)
            {
                next = rule.Resolve(context, next);
            }

            return next;
        }
    }
}
