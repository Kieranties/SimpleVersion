// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Diagnostics;
using SimpleVersion.Extensions;

namespace SimpleVersion
{
    /// <summary>
    /// Simple exception handling asserts.
    /// </summary>
    public static class Assert
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
        public static T ArgumentNotNull<T>(T value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            return value;
        }

        /// <summary>
        /// Asserts the given value is not null.
        /// </summary>
        /// <typeparam name="T">The type of object being asserted.</typeparam>
        /// <param name="value">The value to be asserted.</param>
        /// <param name="message">The optional exception message.</param>
        /// <exception cref="NullReferenceException">Thrown if the given value is null.</exception>
        /// <returns>The value if not null.</returns>
        [DebuggerStepThrough]
        public static T NotNull<T>(T value, string? message = null)
        {
            if (value == null)
            {
                message = message.DefaultIfNullOrWhiteSpace(Resources.Exception_UnexpectedNull);
                throw new NullReferenceException(message);
            }

            return value;
        }
    }
}
