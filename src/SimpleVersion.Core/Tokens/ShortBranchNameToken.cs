// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Legacy handler for short branch name.
    /// </summary>
    public class ShortBranchNameToken : BranchNameToken
    {
        /// <inheritdoc/>
        public override string Key => Options.Short + base.Key;

        /// <inheritdoc/>
        public override bool SupportsOptions => false;

        /// <inheritdoc/>
        public override string Evaluate(IVersionContext context, ITokenEvaluator evaluator)
        {
            return EvaluateWithOptionImpl(Options.Short, context, evaluator);
        }
    }
}
