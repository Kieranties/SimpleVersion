// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles the formatting of the version string.
    /// </summary>
    public class VersionToken : BaseToken
    {
        public static class Options
        {
            public const string Major = "M";
            public const string Minor = "m";
            public const string Patch = "p";
            public const string RevisionIfSet = "r";
            public const string Revision = "R";
            public const string Default = "Mmpr";
        }

        /// <inheritdoc/>
        public override string Key => "version";

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
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(optionValue, nameof(optionValue));

            var versionParts = new List<int>();

            foreach (var c in optionValue)
            {
                int num;

                if (c == 'r' && context.Result.Revision < 1)
                {
                    continue;
                }

                num = c switch
                {
                    'M' => context.Result.Major,
                    'm' => context.Result.Minor,
                    'p' => context.Result.Patch,
                    'r' => context.Result.Revision,
                    'R' => context.Result.Revision,
                    _ => throw new InvalidOperationException($"Invalid character '{c}' in version option [{optionValue}].")
                };

                versionParts.Add(num);
            }

            return string.Join('.', versionParts);
        }
    }
}
