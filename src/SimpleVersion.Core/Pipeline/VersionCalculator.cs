// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using SimpleVersion.Model;
using SimpleVersion.Pipeline.BuildServers;
using SimpleVersion.Pipeline.Formatting;
using System;
using System.Collections.Generic;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Entry point for version calculation.
    /// </summary>
    public class VersionCalculator : IVersionCalculator
    {
        private readonly Queue<Lazy<IVersionProcessor>> _queue = new Queue<Lazy<IVersionProcessor>>();

        /// <summary>
        /// Default calculator instance with default processors already populated.
        /// </summary>
        /// <returns>An instance of <see cref="IVersionCalculator"/>.</returns>
        public static IVersionCalculator Default()
            => new VersionCalculator()
                .AddProcessor<ResolveRepositoryPathProcess>()
                .AddProcessor<AzureDevopsProcess>()
                .AddProcessor<ResolveConfigurationProcess>()
                .AddProcessor<VersionFormatProcess>()
                .AddProcessor<Semver1FormatProcess>()
                .AddProcessor<Semver2FormatProcess>();

        /// <inheritdoc/>
        public IVersionCalculator AddProcessor<T>()
            where T : IVersionProcessor, new()
        {
            _queue.Enqueue(new Lazy<IVersionProcessor>(() => new T()));
            return this;
        }

        /// <inheritdoc/>
        public VersionResult GetResult(string path)
        {
            var ctx = new VersionContext { Path = path };

            foreach (var processor in _queue)
                processor.Value.Apply(ctx);

            return ctx.Result;
        }
    }
}
