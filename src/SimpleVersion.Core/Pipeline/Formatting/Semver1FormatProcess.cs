// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Abstractions.Rules;
using SimpleVersion.Rules;

namespace SimpleVersion.Pipeline.Formatting
{
    /// <summary>
    /// Processes the Semver 1 format.
    /// </summary>
    public class Semver1FormatProcess : IVersionContextProcessor
    {
        /// <summary>
        /// The key used to identify this format.
        /// </summary>
        public const string FormatKey = "Semver1";

        /// <inheritdoc/>
        public void Apply(IVersionContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            var rules = new ITokenRule<string>[]
            {
                new HeightTokenRule(true),
                ShortShaTokenRule.Instance,
                BranchNameTokenRule.Instance,
                ShortBranchNameTokenRule.Instance,
                BranchNameSuffixTokenRule.Instance
            };

            var labelParts = context.Configuration.Label.ApplyTokenRules(context, rules);
            var label = string.Join("-", labelParts).ResolveTokenRules(context, rules);

            var format = context.Result.Version;

            if (!string.IsNullOrWhiteSpace(label))
            {
                format += $"-{label}";
            }

            context.Result.Formats[FormatKey] = format;
        }
    }
}
