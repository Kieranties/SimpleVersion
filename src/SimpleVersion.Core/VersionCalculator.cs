// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;
using System.IO;
using SimpleVersion.Pipeline;

namespace SimpleVersion
{
    /// <summary>
    /// Entry point for version calculation.
    /// </summary>
    public class VersionCalculator : IVersionCalculator
    {
        private readonly string _path;
        private readonly IEnumerable<IVersionProcessor> _processors;
        private readonly ISerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionCalculator"/> class.
        /// </summary>
        /// <param name="path">The path for the repository.</param>
        /// <param name="processors">The processors to generate version info.</param>
        /// <param name="serializer">The serializer to write output.</param>
        public VersionCalculator(
            string path,
            IEnumerable<IVersionProcessor> processors,
            ISerializer serializer)
        {
            _path = path;
            _processors = Assert.ArgumentNotNull(processors, nameof(processors));
            _serializer = Assert.ArgumentNotNull(serializer, nameof(serializer));
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
