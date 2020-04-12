// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.IO;
using LibGit2Sharp;
using SimpleVersion.Environment;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;
using SimpleVersion.Serialization;

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
            var serializer = new Serializer();
            var repository = new GitVersionRepository(path, serializer);
            var environmentVariables = new EnvironmentVariableAccessor();

            // TODO: Resolve environment
            var environment = new AzureDevopsEnvironment(environmentVariables);

            // TODO: Populate processors
            var processors = new IVersionPipelineProcessor[]
            {
                new VersionFormatProcessor(),
                new Semver1FormatProcessor(),
                new Semver2FormatProcessor()
            };

            // TODO: Apply token rules ahead of format processing
            var pipeline = new VersionPipeline(environment, repository, processors);

            return pipeline.Process();
        }
    }
}
