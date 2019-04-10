// Licensed under the MIT license. See https://kieranties.mit-license.org/ for full license information.

using System;

namespace SimpleVersion
{
    public static class StringExtensions
    {
        public static bool ToBool(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (int.TryParse(value, out var intValue))
            {
                return intValue == 0;
            }

            return bool.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
