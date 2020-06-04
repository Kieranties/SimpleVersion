// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System.Collections.Generic;
using System.Linq;
using SimpleVersion.Environment;

namespace SimpleVersion.Pipeline
{
    /// <summary>
    /// Resolves the environment and applies to a <see cref="VersionContext"/>.
    /// </summary>
    public class EnvironmentVersionProcessor : IVersionProcessor
    {
        private readonly IEnumerable<IVersionEnvironment> _environments;
        private readonly DefaultVersionEnvironment _fallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentVersionProcessor"/> class.
        /// </summary>
        /// <param name="environments">The environments that may apply.</param>
        public EnvironmentVersionProcessor(
            IEnumerable<IVersionEnvironment> environments,
            DefaultVersionEnvironment fallbackEnvironment)
        {
            _environments = Assert.ArgumentNotNull(environments, nameof(environments));
            _fallback = Assert.ArgumentNotNull(fallbackEnvironment, nameof(fallbackEnvironment));
        }

        /// <inheritdoc/>
        public void Process(IVersionContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            context.Environment = _environments.FirstOrDefault(x => x.IsValid) ?? _fallback;
        }
    }
}
