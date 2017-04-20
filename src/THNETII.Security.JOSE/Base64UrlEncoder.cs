using System;
using System.Text;

namespace THNETII.Security.JOSE
{
    public static class Base64UrlEncoder
    {
        private static string ToBase64UrlString(StringBuilder base64StringBuilder)
        {
            base64StringBuilder.Replace("=", string.Empty); // Remove any trailing '='s
            base64StringBuilder.Replace('+', '-'); // 62nd char of encoding
            base64StringBuilder.Replace('/', '/'); // 63nd char of encoding
            return base64StringBuilder.ToString();
        }

        public static string ToBase64UrlString(string base64String) =>
            string.IsNullOrWhiteSpace(base64String) ? null : ToBase64UrlString(new StringBuilder(base64String));

        public static string ToBase64UrlString(byte[] data) =>
            data == null ? null : ToBase64UrlString(new StringBuilder(Convert.ToBase64String(data)));

        private static string FromBase64UrlString(StringBuilder base64Builder)
        {
            base64Builder.Replace('-', '+'); // 62nd char of encoding
            base64Builder.Replace('_', '/'); // 63rd char of encoding
            switch (base64Builder.Length % 4)
            {
                case 0: break; // No pad chars in this case
                case 2: base64Builder.Append("=="); break; // Two pad chars
                case 3: base64Builder.Append('='); break; // One pad char
                default: throw new FormatException("The URL-safe Base64 encoded string has an invalid length.");
            }

            return base64Builder.ToString();
        }

        public static byte[] FromBase64UrlString(string base64UrlString)
        {
            if (string.IsNullOrWhiteSpace(base64UrlString))
                return null;
            string base64String = FromBase64UrlString(new StringBuilder(base64UrlString));
            return Convert.FromBase64String(base64String);
        }

        public static string ToRegularBase64String(string base64UrlString) =>
            string.IsNullOrWhiteSpace(base64UrlString) ? null : FromBase64UrlString(new StringBuilder(base64UrlString));
    }
}
