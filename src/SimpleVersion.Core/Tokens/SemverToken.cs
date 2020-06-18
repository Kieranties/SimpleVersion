// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Linq;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of a Semver version.
    /// </summary>
    public class SemverToken : BaseToken
    {
        public static class Options
        {
            public const string Semver1 = "1";
            public const string Semver2 = "2";
            public const string Default = Semver2;
        }

        /// <inheritdoc/>
        public override string Key => "semver";

        /// <inheritdoc/>
        public override bool SupportsOptions => true;

        /// <inheritdoc/>
        public override string Evaluate(IVersionContext context, ITokenEvaluator evaluator)
        {
            return EvaluateWithOption(Options.Default, context, evaluator);
        }

        /// <inheritdoc/>
        protected override string EvaluateWithOptionImpl(string optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(optionValue, nameof(optionValue));
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            var joinChar = optionValue switch
            {
                Options.Semver1 => "-",
                Options.Semver2 => ".",
                _ => throw new InvalidOperationException($"'{optionValue}' is not a valid semver version")
            };

            var version = evaluator.Process<VersionToken>(context);
            var label = evaluator.Process<LabelToken>(joinChar, context);

            // TODO: move checks for token to evaluator

            // if we have a label and it does not contain the height, it needs to be added
            var needsHeight = !string.IsNullOrEmpty(label) && !context.Configuration.Label.Any(x => x.Contains("*", StringComparison.OrdinalIgnoreCase));
            if (needsHeight)
            {
                var padding = optionValue == Options.Semver1 ? "4" : HeightToken.Options.Default;
                var height = evaluator.Process<HeightToken>(padding, context);
                label = string.Join(joinChar, label, height);
            }

            // if we have is release and it does not contain the sha
            var needsSha = !context.Result.IsRelease && context.Configuration.Label.Any(x => x.Contains("sha", System.StringComparison.OrdinalIgnoreCase)); // TODO: not explicit enough
            if (needsSha)
            {
                var sha = evaluator.Process<ShortShaToken>(context);
                label = string.Join(joinChar, label, sha);
            }

            var result = version;

            if (!string.IsNullOrWhiteSpace(label))
            {
                result += $"-{label}";
            }

            if (optionValue == Options.Semver2)
            {
                var metadata = evaluator.Process<MetadataToken>(context);
                if (!string.IsNullOrWhiteSpace(metadata))
                {
                    result += $"+{metadata}";
                }
            }

            return result;
        }
    }
}
