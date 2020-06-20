// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Linq;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of label parts.
    /// </summary>
    [Token(_tokenKey, DefaultOption = _dotOption, Description = "Provides parsing of the version label.")]
    [TokenValueOption(_dotOption, Description = "Joins the label parts into a string separated with the '.' character.")]
    [TokenFallbackOption("Joins the label parts into a string separated with the given value.")]
    public class LabelToken : IToken
    {
        private const string _tokenKey = "label";
        private const string _dotOption = ".";

        /// <inheritdoc/>
        public string Evaluate(string optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(optionValue, nameof(optionValue));
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            var parts = context.Configuration.Label.Select(l => evaluator.Process(l, context));
            return string.Join(optionValue, parts);
        }
    }
}
