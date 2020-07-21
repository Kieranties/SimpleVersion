// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Linq;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of label parts.
    /// </summary>
    [TokenValueOption(_dotOption, Description = "Joins the label parts into a string separated with the '.' character.")]
    [TokenFallbackOption("Joins the label parts into a string separated with the given value.")]
    public class LabelToken : ITokenRequestHandler<LabelTokenRequest>
    {
        private const string _dotOption = ".";

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

        public void Parse(string optionValue)
        {

        }
    }
}
