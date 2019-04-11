// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion
{
    /// <summary>
    /// Enables access to environment variables.
    /// </summary>
    public class VersioningEnvironment : IEnvironment
    {
        /// <inheritdoc/>
        public string GetVariable(string name) => System.Environment.GetEnvironmentVariable(name);
    }
}
