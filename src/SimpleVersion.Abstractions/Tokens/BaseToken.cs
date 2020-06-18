// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Base abstraction to implement tokens.
    /// </summary>
    public abstract class BaseToken : IToken
    {
        /// <inheritdoc/>
        public abstract string Key { get; }

        /// <inheritdoc/>
        public virtual bool SupportsOptions => false;

        /// <inheritdoc/>
        public abstract string Evaluate(IVersionContext context, ITokenEvaluator evaluator);

        /// <inheritdoc/>
        public string EvaluateWithOption(string optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            return SupportsOptions
                ? EvaluateWithOptionImpl(optionValue, context, evaluator)
                : throw new InvalidOperationException($"Token '{Key}' does not support options.");
        }

        /// <summary>
        /// Inner implementation for handling valuation with options.
        /// </summary>
        /// <param name="optionValue">The option value to modify the result.</param>
        /// <param name="context">The current version context.</param>
        /// <param name="evaluator">The <see cref="ITokenEvaluator"/> for chained evaluation.</param>
        /// <returns>A value resolved for this token.</returns>
        protected virtual string EvaluateWithOptionImpl(string optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            throw new NotImplementedException();
        }
    }
}
