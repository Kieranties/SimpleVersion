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
        /// <param name="value">The value to be asserted.</param>
        /// <param name="name">The name of the argument to be asserted.</param>
        /// <exception cref="ArgumentNullException">Thrown if the given value is null.</exception>
        [DebuggerStepThrough]
        public static void ArgumentNotNull(object? value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// Asserts the given value is not null.
        /// </summary>
        /// <param name="value">The value to be asserted.</param>
        /// <param name="message">The optional exception message.</param>
        /// <exception cref="NullReferenceException">Thrown if the given value is null.</exception>
        [DebuggerStepThrough]
        public static void NotNull(object? value, string? message = null)
        {
            message = message.DefaultIfNullOrWhiteSpace(Resources.Exception_UnexpectedNull);

            if (value == null)
            {
                throw new NullReferenceException(message);
            }
        }

        /// <summary>
        /// Asserts the given value is not null.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Exception"/> to be returned.</typeparam>
        /// <param name="value">The value to be asserted.</param>
        /// <param name="message">The optional exception message.</param>
        [DebuggerStepThrough]
        public static void NotNull<T>(object? value, string? message = null)
            where T : Exception
        {
            message = message.DefaultIfNullOrWhiteSpace(Resources.Exception_UnexpectedNull);

            try
            {
                NotNull(value);
            }
            catch (Exception ex)
            {
                var wrapped = (Exception)Activator.CreateInstance(typeof(T), message, ex);
                throw wrapped;
            }
        }
    }
}
