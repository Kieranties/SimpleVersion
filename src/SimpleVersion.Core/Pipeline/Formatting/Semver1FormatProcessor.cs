// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Rules;

namespace SimpleVersion.Pipeline.Formatting
{
    /// <summary>
    /// Processes the Semver 1 format.
    /// </summary>
    public class Semver1FormatProcessor : IVersionProcessor
    {
        /// <summary>
        /// The key used to identify this format.
        /// </summary>
        public const string FormatKey = "Semver1";

        /// <inheritdoc/>
        public void Process(IVersionContext context)
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
