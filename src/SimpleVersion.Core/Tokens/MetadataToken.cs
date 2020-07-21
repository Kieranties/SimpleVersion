// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Linq;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of metadata parts.
    /// </summary>
    [TokenValueOption(_dotOption, Description = "Joins the metadata parts into a string separated with the '.' character.")]
    [TokenFallbackOption("Joins the metadata parts into a string separated with the given value.")]
    public class MetadataToken : ITokenRequestHandler<MetadataTokenRequest>
    {
        private const string _tokenKey = "metadata";
        private const string _dotOption = ".";

        /// <inheritdoc/>
        public string Evaluate(MetadataTokenRequest request, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(request, nameof(request));
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            var parts = context.Configuration.Metadata.Select(l => evaluator.Parse(l, context));
            return string.Join(request.Separator, parts);
        }
    }


    [Token("metadata", Description = "Provides parsing of the version metadata.")]
    public class MetadataTokenRequest : ITokenRequest
    {
        public string Separator { get; set; } = ".";

        public void Parse(string optionValue)
        {

        }
    }
}
