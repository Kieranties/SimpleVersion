// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles the collection of the version string.
    /// </summary>
    public class VersionTokenHandler : ITokenHandler
    {
        /// <inheritdoc/>
        public string Key => "version";

        /// <inheritdoc/>
        public string Process(string? optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            if (string.IsNullOrWhiteSpace(optionValue))
            {
                optionValue = "Mmpr";
            }

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
