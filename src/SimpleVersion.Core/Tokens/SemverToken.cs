// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Linq;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of a Semver version.
    /// </summary>
    public class SemverToken : ITokenRequestHandler<SemverTokenRequest>
    {
        /// <inheritdoc/>
        public string Evaluate(SemverTokenRequest request, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(request, nameof(request));
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            var joinChar = request.Version switch
            {
                1 => "-",
                2 => ".",
                _ => throw new InvalidOperationException($"'{request.Version}' is not a valid semver version")
            };

            var version = evaluator.Process(new VersionTokenRequest(), context);
            var label = evaluator.Process(new LabelTokenRequest { Separator = joinChar }, context);

            // TODO: move checks for token to evaluator

            // if we have a label and it does not contain the height, it needs to be added
            var needsHeight = !string.IsNullOrEmpty(label) && !context.Configuration.Label.Any(x => x.Contains("*", StringComparison.OrdinalIgnoreCase));
            if (needsHeight)
            {
                var height = request.Version switch
                {
                    1 => evaluator.Process(new HeightTokenRequest { Padding = 4 }, context),
                    _ => evaluator.Process(new HeightTokenRequest(), context)
                }; 
                label = string.Join(joinChar, label, height);
            }

            // if we have is release and it does not contain the sha
            var needsSha = !context.Result.IsRelease && context.Configuration.Label.Any(x => x.Contains("sha", System.StringComparison.OrdinalIgnoreCase)); // TODO: not explicit enough
            if (needsSha)
            {
                var sha = evaluator.Process(new ShaTokenRequest { Length = 7 }, context);
                label = string.Join(joinChar, label, sha);
            }

            var result = version;

            if (!string.IsNullOrWhiteSpace(label))
            {
                result += $"-{label}";
            }

            if (request.Version == 2)
            {
                var metadata = evaluator.Process(new MetadataTokenRequest(), context);
                if (!string.IsNullOrWhiteSpace(metadata))
                {
                    result += $"+{metadata}";
                }
            }

            return result;
        }
    }


    [Token("semver", Description = "Provides parsing of full semver compatible versions.")]
    public class SemverTokenRequest : ITokenRequest
    {
        public int Version { get; set; } = 2;

        public void Parse(string optionValue)
        {
            if(int.TryParse(optionValue, out var result))
            {
                if(result < 1 || result > 2)
                {
                    throw new ArgumentOutOfRangeException("Values must be 1 or 2");
                }

                Version = result;
            }

            throw new ArgumentException("Invalid value");
        }
    }
}
