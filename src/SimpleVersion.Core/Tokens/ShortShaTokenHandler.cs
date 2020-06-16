// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// ELegacy handler for short sha token.
    /// </summary>
    public class ShortShaTokenHandler : ITokenHandler
    {
        /// <inheritdoc/>
        public string Key => "shortsha";

        /// <inheritdoc/>
        public string Process(string? option, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            return evaluator.Process("c{sha:7}", context);
        }
    }
}
