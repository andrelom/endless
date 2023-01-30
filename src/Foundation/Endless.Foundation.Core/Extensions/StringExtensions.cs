using System;

namespace Endless.Foundation.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidGuid(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return Guid.TryParse(value, out var _);
        }
    }
}
