// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.IO;
using SimpleVersion.Environment;
using SimpleVersion.Pipeline;
using SimpleVersion.Pipeline.Formatting;
using SimpleVersion.Serialization;
using SimpleVersion.Tokens;

namespace SimpleVersion
{
    /// <summary>
    /// Entry point for version calculation.
    /// </summary>
    public class VersionCalculator : IVersionCalculator
    {
        private static readonly Serializer _serializer = new Serializer();
        private static readonly EnvironmentVariableAccessor _environmentVariables = new EnvironmentVariableAccessor();
        private static readonly IVersionEnvironment[] _environments = new IVersionEnvironment[]
        {
            new AzureDevopsEnvironment(_environmentVariables),
            new DefaultVersionEnvironment(_environmentVariables)
        };

        private static readonly ITokenEvaluator _evaluator = new TokenEvaluator(new ITokenHandler[]
        {
            new BranchNameTokenHandler(),
            new LabelTokenHandler(),
            new PrTokenHandler(),
            new SemverTokenHandler(),
            new ShaTokenHandler(),
            new ShortBranchNameTokenHandler(),
            new VersionTokenHandler()
        });

        private static readonly IVersionProcessor[] _processors = new IVersionProcessor[]
        {
            new EnvironmentVersionProcessor(_environments),
            new GitRepositoryVersionProcessor(_serializer),
            new VersionVersionProcessor(_evaluator),
            new FormatsVersionProcessor(_evaluator)
        };

        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionCalculator"/> class.
        /// </summary>
        /// <param name="path">The path for the repository.</param>
        public VersionCalculator(string path)
        {
            _path = path;
        }

        /// <inheritdoc/>
        public VersionResult GetResult()
        {
            var ctx = new VersionContext(_path);
            foreach (var proc in _processors)
            {
                proc.Process(ctx);
            }

            return ctx.Result;
        }

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
