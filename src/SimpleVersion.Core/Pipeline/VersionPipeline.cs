// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;
using System.Linq;
using SimpleVersion.Environment;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Handles version requests.
    /// </summary>
    public class VersionPipeline : IVersionPipeline
    {
        private readonly IVersionEnvironment _environment;
        private readonly IVersionRepository _repository;
        private readonly IVersionPipelineProcessor[] _processors;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionPipeline"/> class.
        /// </summary>
        /// <param name="environment">The <see cref="IVersionEnvironment"/> for requests.</param>
        /// <param name="repository">The <see cref="IVersionRepository"/> for requests.</param>
        /// <param name="processors">The <see cref="IVersionPipelineProcessor"/> processors for requests.</param>
        public VersionPipeline(
            IVersionEnvironment environment,
            IVersionRepository repository,
            IEnumerable<IVersionPipelineProcessor> processors)
        {
            // TODO: Resolve environment properly
            _environment = Assert.ArgumentNotNull(environment, nameof(environment));
            _repository = Assert.ArgumentNotNull(repository, nameof(repository));
            _processors = Assert.ArgumentNotNull(processors, nameof(processors)).ToArray();
        }

        /// <inheritdoc/>
        public VersionResult Process()
        {
            var context = new VersionContext(_environment, _repository);

            foreach (var processor in _processors)
            {
                processor.Process(context);
            }

            return context.Result;
        }
    }
}
