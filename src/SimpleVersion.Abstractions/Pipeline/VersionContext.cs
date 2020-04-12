// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Configuration;
using SimpleVersion.Environment;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Enapsulates state for a version request.
    /// </summary>
    public class VersionContext : IVersionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionContext"/> class.
        /// </summary>
        /// <param name="environment">The <see cref="IVersionEnvironment"/> for the request.</param>
        /// <param name="repository">The <see cref="IVersionRepository"/> for the request.</param>
        public VersionContext(
            IVersionEnvironment environment,
            IVersionRepository repository)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            Result = new VersionResult();
            environment.Process(this);
            Configuration = repository.GetConfiguration(Result.CanonicalBranchName);
            repository.Process(this);
        }

        /// <inheritdoc/>
        public IVersionEnvironment Environment { get; }

        /// <inheritdoc/>
        public VersionConfiguration Configuration { get; }

        /// <inheritdoc/>
        public VersionResult Result { get; }
    }
}
