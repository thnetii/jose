using System;

namespace THNETII.Security.JOSE
{
    internal static class UriStringConverter
    {
        internal static Uri ParseOrDefault(string uriString)
        {
            if (uriString == null)
                return null;
            try { return new Uri(uriString); }
            catch (UriFormatException) { return null; }
        }

        internal static string ToString(Uri uri) => uri?.OriginalString;
    }
}