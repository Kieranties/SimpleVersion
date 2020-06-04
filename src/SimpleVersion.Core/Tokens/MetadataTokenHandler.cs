// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Linq;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of metadata parts.
    /// </summary>
    public class MetadataTokenHandler : ITokenHandler
    {
        /// <inheritdoc/>
        public string Key => "metadata";

        /// <inheritdoc/>
        public string Process(string? optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            if (optionValue == null)
            {
                optionValue = ".";
            }

            return string.Join(optionValue, context.Configuration.Metadata
                .Select(l => evaluator.Process(l, context))
                .ToList());
        }
    }
}
