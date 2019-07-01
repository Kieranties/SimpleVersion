// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Rules;
using System;

namespace SimpleVersion.Pipeline.Formatting
{
    /// <summary>
    /// Processes the version string.
    /// </summary>
    public class VersionFormatProcess : IVersionContextProcessor
    {
        /// <inheritdoc/>
        public void Apply(IVersionContext context)
        {
            var versionString = HeightTokenRule.Instance.Resolve(context, context.Configuration.Version);

            if (Version.TryParse(versionString, out var version))
            {
                context.Result.Major = version.Major > -1 ? version.Major : 0;
                context.Result.Minor = version.Minor > -1 ? version.Minor : 0;
                context.Result.Patch = version.Build > -1 ? version.Build : 0;

                context.Result.Version = $"{context.Result.Major}.{context.Result.Minor}.{context.Result.Patch}";
                if (version.Revision > -1)
                {
                    context.Result.Version += $".{version.Revision}";
                    context.Result.Revision = version.Revision;
                }
            }
            else
            {
                throw new InvalidOperationException(Resources.InvalidVersionFormat.FormatWith(versionString));
            }
        }
    }
}
