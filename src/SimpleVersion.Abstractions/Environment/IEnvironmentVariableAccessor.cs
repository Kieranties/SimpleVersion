// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

namespace SimpleVersion.Environment
{
    /// <summary>
    /// Supports accessing environment variables.
    /// </summary>
    public interface IEnvironmentVariableAccessor
    {
        /// <summary>
        /// Gets the value of the given variable name.
        /// </summary>
        /// <param name="name">The name of the variable to retrieve.</param>
        /// <returns>The value of the given variable.</returns>
        string? GetVariable(string name);
    }
}
