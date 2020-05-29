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
            if (optionValue != null)
            {
                if (!int.TryParse(optionValue, out specVersion))
                {
                    throw new ArgumentException($"Could not parse value {optionValue}", optionValue);
                }
            }

            var joinChar = specVersion switch
            {
                1 => '-',
                2 => '.',
                _ => throw new ArgumentOutOfRangeException(nameof(optionValue))
            };

            var version = evaluator.Process("{version}", context);
            var label = evaluator.Process($"{{label:{joinChar}}}", context);

            var result = version;

            if (!string.IsNullOrWhiteSpace(label))
            {
                result += $"-{label}";
            }

            return result;
        }
    }
}
