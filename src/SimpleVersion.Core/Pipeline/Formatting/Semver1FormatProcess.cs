// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Rules;

namespace SimpleVersion.Pipeline.Formatting
{
    /// <summary>
    /// Processes the Semver 1 format.
    /// </summary>
    public class Semver1FormatProcess : IVersionProcessor
    {
        /// <inheritdoc/>
        public void Apply(VersionContext context)
        {
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
                format += $"-{label}";

            context.Result.Formats["Semver1"] = format;
        }
    }
}
