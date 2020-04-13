// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.IO;
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
        private readonly Serializer _serializer;
        private readonly VersionPipeline _pipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionCalculator"/> class.
        /// </summary>
        /// <param name="path">The path for the repository.</param>
        public VersionCalculator(string path)
        {
            _serializer = new Serializer();
            var repository = new GitVersionRepository(path, _serializer);
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
            _pipeline = new VersionPipeline(environment, repository, processors);
        }

        /// <inheritdoc/>
        public VersionResult GetResult() => _pipeline.Process();

        /// <inheritdoc/>
        public void WriteResult(TextWriter output)
        {
            Assert.ArgumentNotNull(output, nameof(output));

            var result = GetResult();
            var serialized = _serializer.Serialize(result);
            output.WriteLine(serialized);
        }
    }
}
