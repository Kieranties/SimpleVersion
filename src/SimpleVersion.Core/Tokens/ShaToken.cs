// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Exposes the git sha as a token for consumption.
    /// </summary>
    public class ShaToken : BaseToken
    {
        public static class Options
        {
            public const string Full = "full";
            public const string Short = "short";
            public const string Default = Full;
        }

        /// <inheritdoc/>
        public override string Key => "sha";

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

            var result = context.Result.Sha;

            int ParseOptionAsInt()
            {
                if (int.TryParse(optionValue, out var length))
                {
                    if (length < 1)
                    {
                        throw new InvalidOperationException($"Invalid sha option {optionValue}.  Expected an integer greater than 0.");
                    }

                    return Math.Min(length, result.Length);
                }

                throw new InvalidOperationException($"Invalid sha option '{optionValue}'.  Expected an integer greater than 0, 'full', or 'short'.");
            }

            var length = optionValue.ToLowerInvariant() switch
            {
                Options.Default => result.Length,
                Options.Short => 7,
                _ => ParseOptionAsInt()
            };

            return "c" + result.Substring(0, length);
        }
    }
}
