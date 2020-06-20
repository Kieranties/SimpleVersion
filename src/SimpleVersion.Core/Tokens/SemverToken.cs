// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Linq;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of a Semver version.
    /// </summary>
    [Token(_tokenKey, DefaultOption = _v2Option, Description = "Provides parsing of full semver compatible versions.")]
    [TokenValueOption(_v1Option, Description = "Returns a semver v1 formatted string.")]
    [TokenValueOption(_v2Option, Description = "Returns a semver v2 formatted string.")]
    public class SemverToken : IToken
    {
        private const string _tokenKey = "semver";
        internal const string _v1Option = "1";
        internal const string _v2Option = "2";

        /// <inheritdoc/>
        public string Evaluate(string optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(optionValue, nameof(optionValue));
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            var joinChar = optionValue switch
            {
                _v1Option => "-",
                _v2Option => ".",
                _ => throw new InvalidOperationException($"'{optionValue}' is not a valid semver version")
            };

            var version = evaluator.Process<VersionToken>(context);
            var label = evaluator.Process<LabelToken>(joinChar, context);

            // TODO: move checks for token to evaluator

            // if we have a label and it does not contain the height, it needs to be added
            var needsHeight = !string.IsNullOrEmpty(label) && !context.Configuration.Label.Any(x => x.Contains("*", StringComparison.OrdinalIgnoreCase));
            if (needsHeight)
            {
                var height = optionValue switch
                {
                    _v1Option => evaluator.Process<HeightToken>("4", context),
                    _ => evaluator.Process<HeightToken>(context)
                }; 
                label = string.Join(joinChar, label, height);
            }

            // if we have is release and it does not contain the sha
            var needsSha = !context.Result.IsRelease && context.Configuration.Label.Any(x => x.Contains("sha", System.StringComparison.OrdinalIgnoreCase)); // TODO: not explicit enough
            if (needsSha)
            {
                var sha = evaluator.Process<ShaToken>(ShaToken._shortOption, context);
                label = string.Join(joinChar, label, sha);
            }

            var result = version;

            if (!string.IsNullOrWhiteSpace(label))
            {
                result += $"-{label}";
            }

            if (optionValue == _v2Option)
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
