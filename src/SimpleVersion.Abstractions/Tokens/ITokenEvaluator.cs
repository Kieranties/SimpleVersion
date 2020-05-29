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
        string Process(string tokenString, IVersionContext context);
    }
}
