// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Globalization;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles token replacement for height.
    /// </summary>
    public class HeightToken : BaseToken
    {
        public static class Options
        {
            public const string Default = "0";
        }

        /// <inheritdoc />
        public override string Key => "*";

        /// <inheritdoc/>
        public override bool SupportsOptions => true;

        /// <inheritdoc/>
        public override string Evaluate(IVersionContext context, ITokenEvaluator evaluator)
        {
            return EvaluateWithOption(Options.Default, context, evaluator);
        }

        /// <inheritdoc/>
        protected override string EvaluateWithOptionImpl(string optionValue, IVersionContext context, ITokenEvaluator evaluator)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            if (int.TryParse(optionValue, out var padding))
            {
                var height = context.Result.Height.ToString(CultureInfo.InvariantCulture);
                return height.PadLeft(padding, '0');
            }

            throw new InvalidOperationException($"Invalid option for height token: '{optionValue}'");
        }
    }
}
