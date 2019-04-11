// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Rules;

namespace SimpleVersion.Pipeline.Formatting
{
    /// <summary>
    /// Process the Semver 2 format.
    /// </summary>
    public class Semver2FormatProcess : IVersionProcessor
    {
        /// <inheritdoc/>
        public void Apply(VersionContext context)
        {
            var rules = new ITokenRule<string>[]
            {
                HeightTokenRule.Instance,
                ShortShaTokenRule.Instance,
                BranchNameTokenRule.Instance,
                ShortBranchNameTokenRule.Instance,
                BranchNameSuffixTokenRule.Instance
            };

            var labelParts = context.Configuration.Label.ApplyTokenRules(context, rules);
            var label = string.Join(".", labelParts).ResolveTokenRules(context, rules);
            var meta = string.Join(".", context.Configuration.MetaData).ResolveTokenRules(context, rules);

            var format = context.Result.Version;

            if (!string.IsNullOrWhiteSpace(label))
                format += $"-{label}";

            if (!string.IsNullOrWhiteSpace(meta))
                format += $"+{meta}";

            context.Result.Formats["Semver2"] = format;
        }
    }
}
