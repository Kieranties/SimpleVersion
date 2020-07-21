// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Exposes the git sha as a token for consumption.
    /// </summary>
    [TokenValueOption(_fullOption, Description = "Returns the full commit sha.")]
    [TokenValueOption(_shortOption, Alias = _shortOption + _tokenKey, Description = "Returns the first seven characters of the commit sha.")]
    [TokenFallbackOption("Provide a number greater than 0 to return up to that many characters from the commit sha.")]
    public class ShaToken : ITokenRequestHandler<ShaTokenRequest>
    {
        private const string _tokenKey = "sha";
        private const string _fullOption = "full";
        private const string _shortOption = "short";

        /// <inheritdoc/>
        public string Evaluate(ShaTokenRequest request, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(request, nameof(request));
            Assert.ArgumentNotNull(context, nameof(context));

            var result = context.Result.Sha;

            return "c" + result.Substring(0, request.Length);
        }
    }

    [Token("sha", Description = "Provides parsing of the commit sha.")]
    public class ShaTokenRequest : ITokenRequest
    {
        // TODO: handle 'short', 'full', greater than 0;
        public int Length { get; set; } = int.MaxValue;

        public void Parse(string optionValue)
        {
            // TODO: implement parsing / validation
        }
    }

    [Token("shortsha", Description = "Provides parsing of the commit sha.")]
    public class ShortShaTokenRequest : ShaTokenRequest
    {
        public ShortShaTokenRequest()
        {
            Length = 7;
        }
    }
}
