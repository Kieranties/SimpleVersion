// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Tokens;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Handles processing of formats for the version request.
    /// </summary>
    public class FormatsVersionProcessor : IVersionProcessor
    {
        private readonly ITokenEvaluator _evaluator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatsVersionProcessor"/> class.
        /// </summary>
        /// <param name="evaluator">The <see cref="ITokenEvaluator"/> to evaluate formats.</param>
        public FormatsVersionProcessor(ITokenEvaluator evaluator)
        {
            _evaluator = evaluator;
        }

        /// <inheritdoc/>
        public void Process(IVersionContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));
            context.Result.Formats["semver1"] = _evaluator.Process("{semver:1}", context);
            context.Result.Formats["semver2"] = _evaluator.Process("{semver:2}", context);
        }
    }
}
