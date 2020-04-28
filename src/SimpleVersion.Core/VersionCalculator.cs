// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.IO;
using System.Linq;
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
        private static readonly EnvironmentVariableAccessor _environmentVariables = new EnvironmentVariableAccessor();
        private static readonly IVersionEnvironment[] _environments = new IVersionEnvironment[]
        {
            new AzureDevopsEnvironment(_environmentVariables),
            new DefaultVersionEnvironment(_environmentVariables)
        };

        private static readonly IVersionProcessor[] _processors = new IVersionProcessor[]
        {
            new VersionFormatProcessor(),
            new Semver1FormatProcessor(),
            new Semver2FormatProcessor()
        };

        private readonly Serializer _serializer;
        private readonly GitVersionRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionCalculator"/> class.
        /// </summary>
        /// <param name="path">The path for the repository.</param>
        public VersionCalculator(string path)
        {
            _serializer = new Serializer();
            var environment = _environments.First(x => x.IsValid);
            _repository = new GitVersionRepository(path, environment, _serializer, _processors);
        }

        /// <inheritdoc/>
        public VersionResult GetResult() => _repository.GetResult();

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
