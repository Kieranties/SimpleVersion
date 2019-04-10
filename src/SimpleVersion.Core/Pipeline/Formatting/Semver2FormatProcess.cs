// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Rules;

namespace SimpleVersion.Pipeline.Formatting
{
    public class Semver2FormatProcess : ICalculatorProcess
    {
        public void Apply(VersionContext context)
        {
            var rules = new IRule<string>[]
            {
                HeightRule.Instance,
                ShortShaRule.Instance,
                BranchNameRule.Instance,
                ShortBranchNameRule.Instance,
                BranchNameSuffixRule.Instance
            };

            var labelParts = context.Configuration.Label.ApplyRules(context, rules);
            var label = string.Join(".", labelParts).ResolveRules(context, rules);
            var meta = string.Join(".", context.Configuration.MetaData).ResolveRules(context, rules);

            var format = context.Result.Version;

            if (!string.IsNullOrWhiteSpace(label))
                format += $"-{label}";

            if (!string.IsNullOrWhiteSpace(meta))
                format += $"+{meta}";

            context.Result.Formats["Semver2"] = format;
        }
    }
}
