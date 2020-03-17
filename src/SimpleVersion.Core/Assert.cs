// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SimpleVersion
{
    /// <summary>
    /// Simple exception handling asserts.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class Assert
    {
        /// <summary>
        /// Asserts the given argument value is not null.
        /// </summary>
        /// <typeparam name="T">The type of object being asserted.</typeparam>
        /// <param name="value">The value to be asserted.</param>
        /// <param name="name">The name of the argument to be asserted.</param>
        /// <exception cref="ArgumentNullException">Thrown if the given value is null.</exception>
        /// <returns>The value if not null.</returns>
        [DebuggerStepThrough]
        internal static T ArgumentNotNull<T>(T value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            return value;
        }
    }
}
