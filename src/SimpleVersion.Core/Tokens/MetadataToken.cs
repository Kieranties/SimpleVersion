// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Linq;
using SimpleVersion.Pipeline;

namespace SimpleVersion.Tokens
{
    /// <summary>
    /// Handles formatting of metadata parts.
    /// </summary>
    public class MetadataToken : BaseToken
    {
        public static class Options
        {
            public const string Default = ".";
        }

        /// <inheritdoc/>
        public override string Key => "metadata";

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
            Assert.ArgumentNotNull(optionValue, nameof(optionValue));
            Assert.ArgumentNotNull(context, nameof(context));
            Assert.ArgumentNotNull(evaluator, nameof(evaluator));

            var parts = context.Configuration.Metadata.Select(l => evaluator.Process(l, context));
            return string.Join(optionValue, parts);
        }
    }
}
