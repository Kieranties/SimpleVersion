// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleVersion
{
    /// <summary>
    /// Contract for accessing environment variables.
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// Gets the value of the given variable name.
        /// </summary>
        /// <param name="name">The name of the variable to retrieve.</param>
        /// <returns>The value of the given variable.</returns>
        string GetVariable(string name);
    }
}
