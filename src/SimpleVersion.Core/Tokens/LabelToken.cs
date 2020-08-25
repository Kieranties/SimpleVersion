// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Linq;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of label parts.
    /// </summary>
    public class LabelToken : ITokenRequestHandler<LabelTokenRequest>
    {
        /// <inheritdoc/>
        public string Evaluate(LabelTokenRequest request, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(request, nameof(request));
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            var parts = context.Configuration.Label.Select(l => evaluator.Parse(l, context));
            return string.Join(request.Separator, parts);
        }
    }

    [Token("label", Description = "Provides parsing of the version label.")]
    public class LabelTokenRequest : ITokenRequest
    {
        public string Separator { get; set; } = ".";

        public void Parse(string optionValue) => this.Separator = optionValue;
    }
}
