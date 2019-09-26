// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;
using System.Globalization;

namespace SimpleVersion
{
    /// <summary>
    /// Extension methods to handle strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string value to a boolean.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>True if 0 or "True" (case insensitive), otherwise false.</returns>
        public static bool ToBool(this string value)
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
