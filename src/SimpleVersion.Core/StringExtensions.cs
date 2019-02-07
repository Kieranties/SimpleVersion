using System;

namespace SimpleVersion
{
    public static class StringExtensions
    {
        public static bool ToBool(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if(int.TryParse(value, out var intValue))
            {
                return intValue == 0;
            }

            return bool.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
