// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Abstractions;
using SimpleVersion.Abstractions.Pipeline;
using SimpleVersion.Model;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;

namespace SimpleVersion
{
    /// <summary>
    /// Entry point for version calculation.
    /// </summary>
    public class VersionCalculator : IVersionCalculator
    {
        /// <summary>
        /// Default calculator instance.
        /// </summary>
        /// <returns>An instance of <see cref="VersionCalculator"/>.</returns>
        public static VersionCalculator Default() => new VersionCalculator();

        /// <inheritdoc/>
        public VersionResult GetResult(string path)
        {
            var ctx = new VersionContext(path);

            // Resolve build server information
            ApplyProcessor<AzureDevopsContextProcessor>(ctx);
            ApplyProcessor<ConfigurationContextProcessor>(ctx);
            ApplyProcessor<VersionFormatProcess>(ctx);
            ApplyProcessor<Semver1FormatProcess>(ctx);
            ApplyProcessor<Semver2FormatProcess>(ctx);

            return ctx.Result;

        }

        private void ApplyProcessor<T>(IVersionContext context)
            where T : IVersionContextProcessor<IVersionContext>, new()
        {
            var instance = new T();
            instance.Apply(context);
        }

        private void ApplyProcessor<T>(VersionContext context)
            where T : IVersionContextProcessor<VersionContext>, new()
        {
            var instance = new T();
            instance.Apply(context);
        }
    }
}
