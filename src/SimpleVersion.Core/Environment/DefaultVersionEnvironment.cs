// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Environment
{
    /// <summary>
    /// Fallback version environment.
    /// </summary>
    public class DefaultVersionEnvironment : BaseVersionEnvironment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultVersionEnvironment"/> class.
        /// </summary>
        /// <param name="accessor">The accessor for environment variables.</param>
        public DefaultVersionEnvironment(IEnvironmentVariableAccessor accessor) : base(accessor)
        {
        }

        /// <inheritdoc/>
        public override bool IsValid => true;
    }
}
