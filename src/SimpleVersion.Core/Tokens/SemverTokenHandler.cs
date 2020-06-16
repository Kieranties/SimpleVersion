// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of a Semver version.
    /// </summary>
    public class SemverTokenHandler : ITokenHandler
    {
        /// <inheritdoc/>
        public string Key => "semver";

        /// <inheritdoc/>
        public string Process(string? optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            var specVersion = 2;
            if (!string.IsNullOrWhiteSpace(optionValue))
            {
                if (!int.TryParse(optionValue, out specVersion))
                {
                    throw new InvalidOperationException($"Could not parse value semver option {optionValue}");
                }
            }

            var joinChar = specVersion switch
            {
                1 => '-',
                2 => '.',
                _ => throw new InvalidOperationException($"'{optionValue}' is not a valid semver version")
            };

            var version = evaluator.Process("{version}", context);
            var label = evaluator.Process($"{{label:{joinChar}}}", context);

            var result = version;

            if (!string.IsNullOrWhiteSpace(label))
            {
                result += $"-{label}";
            }

            if (specVersion == 2)
            {
                var metadata = evaluator.Process("{metadata:.}", context);
                if (!string.IsNullOrWhiteSpace(metadata))
                {
                    result += $"+{metadata}";
                }
            }

            return result;
        }
    }
}
