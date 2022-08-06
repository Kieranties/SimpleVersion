// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Environment
{
    /// <summary>
    /// Exposes access to environment variables.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class EnvironmentVariableAccessor : IEnvironmentVariableAccessor
    {
        /// <inheritdoc/>
        public string? GetVariable(string name) => System.Environment.GetEnvironmentVariable(name);
    }
}
