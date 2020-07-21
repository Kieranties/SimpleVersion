// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Supports evaluation of a string to parse tokens.
    /// </summary>
    public interface ITokenEvaluator
    {
        /// <summary>
        /// Evaluates a string to return the formatted token value.
        /// </summary>
        /// <param name="tokenString">The string which contains tokens.</param>
        /// <param name="context">The <see cref="IVersionContext"/> for the current request.</param>
        /// <returns>The resolved string value.</returns>
        string Parse(string tokenString, IVersionContext context);

        /// <summary>
        /// Evaluates the context for a specific token.
        /// </summary>
        /// <param name="request">The token request to be completed.</param>
        /// <param name="context">The <see cref="IVersionContext"/> for the current request.</param>
        /// <returns>The resolved string value.</returns>
        public string Process(ITokenRequest request, IVersionContext context);
    }
}
