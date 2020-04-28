// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using SimpleVersion.Configuration;
using SimpleVersion.Environment;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Encapsulates state for a version request.
    /// </summary>
    public class VersionContext : IVersionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionContext"/> class.
        /// </summary>
        /// <param name="environment">The <see cref="IVersionEnvironment"/> for the invocation.</param>
        /// <param name="configuration">The <see cref="VersionConfiguration"/> for the current branch.</param>
        /// <param name="result">The <see cref="VersionResult"/> to collect final version details.</param>
        public VersionContext(
            IVersionEnvironment environment,
            VersionConfiguration configuration,
            VersionResult result)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        /// <inheritdoc/>
        public IVersionEnvironment Environment { get; }

        /// <inheritdoc/>
        public VersionConfiguration Configuration { get; }

        /// <inheritdoc/>
        public VersionResult Result { get; }
    }
}
