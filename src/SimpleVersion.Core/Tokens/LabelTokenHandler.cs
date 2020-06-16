// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Linq;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of label parts.
    /// </summary>
    public class LabelTokenHandler : ITokenHandler
    {
        /// <inheritdoc/>
        public string Key => "label";

        /// <inheritdoc/>
        public string Process(string? optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            if (optionValue == null)
            {
                optionValue = ".";
            }

            // TODO: Need logic to identify/add tokens into a string through strong typing
            // TODO: Identify better place for application of logic to label changes
            var parts = context.Configuration.Label;
            var needsHeight = !context.Configuration.Label.Any(x => x.Contains("*", System.StringComparison.OrdinalIgnoreCase)); // TODO: not explicit enough                       
            if (needsHeight) { parts.Add("*"); }

            var needsSha = !context.Result.IsRelease && context.Configuration.Label.Any(x => x.Contains("sha", System.StringComparison.OrdinalIgnoreCase)); // TODO: not explicit enough
            if (needsSha) { parts.Add("c{sha:7}"); }

            return string.Join(optionValue, parts.Select(l => evaluator.Process(l, context)));
        }
    }
}
