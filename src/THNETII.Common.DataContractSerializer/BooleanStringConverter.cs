using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace THNETII.Common.DataContractSerializer
{
    /// <summary>
    /// Provides conversion methods to convert strings to a boolean value.
    /// </summary>
    public static class BooleanStringConverter
    {
        private static TypeConverter intTypeConverter = TypeDescriptor.GetConverter(typeof(int));

        public static bool Parse(string s)
        {
            try { return bool.Parse(s); }
            catch (Exception) when (TryParseAlternative(s, out bool alternateResult))
            { return alternateResult; }
            catch (FormatException fmtException)
            {
                throw new ArgumentException(fmtException.Message,
                    paramName: nameof(s), fmtException);
            }
        }

        public static bool ParseOrDefault(string s) => ParseOrDefault(s, default);

        public static bool ParseOrDefault(string s, bool @default)
            => TryParse(s, out bool value) ? value : @default;

        public static bool? ParseOrNull(string s)
            => TryParse(s, out bool value) ? (bool?)value : null;

        private static bool TryParseAsInt(string s, out int intValue)
        {
            try
            {
                intValue = (int)intTypeConverter.ConvertFromInvariantString(s);
                return true;
            }
            catch
            {
                intValue = default;
                return false;
            }
        }

        private static bool TryParseAlternative(string s, out bool alternateResult)
        {
            if (s == null)
            {
                alternateResult = false;
                return true;
            }
            if (s.Length < 1)
                goto parsingFailed;
            if (TryParseAsInt(s, out int intValue))
            {
                alternateResult = intValue != 0;
                return true;
            }
            if ("yes".StartsWith(s, StringComparison.OrdinalIgnoreCase))
            {
                alternateResult = true;
                return true;
            }
            else if ("no".StartsWith(s, StringComparison.OrdinalIgnoreCase))
            {
                alternateResult = false;
                return true;
            }

            parsingFailed:
            alternateResult = default;
            return false;
        }

        public static bool TryParse(string s, out bool value)
        {
            return bool.TryParse(s, out value) ||
                TryParseAlternative(s, out value);
        }

        public static string ToString(bool value) => value.ToString();
    }
}
