// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Tokens;

namespace SimpleVersion.Pipeline.Formatting
{
    /// <summary>
    /// Processes the version string.
    /// </summary>
    public class VersionVersionProcessor : IVersionProcessor
    {
        private readonly ITokenEvaluator _evaluator;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionVersionProcessor"/> class.
        /// </summary>
        /// <param name="evaluator">The <see cref="ITokenEvaluator"/> to process tokens.</param>
        public VersionVersionProcessor(ITokenEvaluator evaluator)
        {
            _evaluator = evaluator;
        }

        /// <inheritdoc/>
        public void Process(IVersionContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            var versionString = _evaluator.Process(context.Configuration.Version, context);

            if (Version.TryParse(versionString, out var version))
            {
                context.Result.Major = version.Major > -1 ? version.Major : 0;
                context.Result.Minor = version.Minor > -1 ? version.Minor : 0;
                context.Result.Patch = version.Build > -1 ? version.Build : 0;

                if (version.Revision > -1)
                {
                    context.Result.Revision = version.Revision;
                }
            }
            else
            {
                throw new InvalidOperationException(Resources.Exception_InvalidVersionFormat(versionString));
            }
        }
    }
}
