// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion.Extensions
{
    /// <summary>
    /// Extensions to support string handling.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the current value or a specified default if the value is null or white space.
        /// </summary>
        /// <param name="value">The value to validate to check.</param>
        /// <param name="defaultValue">The value to return as default.</param>
        /// <returns><paramref name="value"/> or <paramref name="defaultValue"/> if <paramref name="value"/> is null or white space.</returns>
        public static string DefaultIfNullOrWhiteSpace(this string? value, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return value;
        }

        /// <summary>
        /// Converts a string value to a boolean.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>True if 0 or "True" (case insensitive), otherwise false.</returns>
        public static bool ToBool(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (int.TryParse(value, out var intValue))
            {
                return intValue == 0;
            }

            return bool.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
