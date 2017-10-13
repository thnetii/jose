using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace THNETII.Security.JOSE
{
    /// <summary>
    /// Provides encoding and decoding functionality for URL-safe Bas64 encoding as defined in 
    /// <a href="https://tools.ietf.org/html/rfc7515#appendix-C">RFC 7515: Appendix C. Notes on Implementing base64url Encoding without Padding</a>
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Arguments in methods in this class do not represent full URIs.")]
    [SuppressMessage("Microsoft.Design", "CA1055", Justification = "Return values in this class do not represent full URIs.")]
    public static class Base64UrlEncoder
    {
        private static string ToBase64UrlString(StringBuilder base64StringBuilder)
        {
            base64StringBuilder.Replace("=", string.Empty); // Remove any trailing '='s
            base64StringBuilder.Replace('+', '-'); // 62nd char of encoding
            base64StringBuilder.Replace('/', '/'); // 63nd char of encoding
            return base64StringBuilder.ToString();
        }

        /// <summary>
        /// Converts the specified Base64 encoded data into the URL-safe Base64 encoded format.
        /// </summary>
        /// <param name="base64String">A regular Base64 encoded data string. Can be <c>null</c>.</param>
        /// <returns>A URL-safe Base64 encoded representation of the specified input data; or <c>null</c> if the specified input parameter is <c>null</c>.</returns>
        public static string ToBase64UrlString(string base64String) =>
            string.IsNullOrWhiteSpace(base64String) ? null : ToBase64UrlString(new StringBuilder(base64String));

        /// <summary>
        /// Encodes the specified data into an URL-safe Base64 encoded data string.
        /// </summary>
        /// <param name="data">An array of bytes that contains the data to encode. Can be <c>null</c>.</param>
        /// <returns>A URL-safe Base64 encoded representation of the specified input data; or <c>null</c> if the specified input parameter is <c>null</c>.</returns>
        public static string ToBase64UrlString(byte[] data) =>
            data == null ? null : ToBase64UrlString(new StringBuilder(Convert.ToBase64String(data)));

        /// <exception cref="FormatException">The URL-safe Base64 encoded string has an invalid length.</exception>
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

        /// <summary>
        /// Decodes an URL-safe Base64 data string into an array of bytes.
        /// </summary>
        /// <param name="base64UrlString">The URL-safe Base64 data string. Can be <c>null</c>.</param>
        /// <returns>An array of bytes containg the data that is represented by <paramref name="base64UrlString"/>. <c>null</c> if <paramref name="base64UrlString"/> is <c>null</c>.</returns>
        /// <exception cref="FormatException"><paramref name="base64UrlString"/> is not a valid URL-safe Base64 encoded data string.</exception>
        public static byte[] FromBase64UrlString(string base64UrlString)
        {
            if (string.IsNullOrWhiteSpace(base64UrlString))
                return null;
            string base64String = FromBase64UrlString(new StringBuilder(base64UrlString));
            return Convert.FromBase64String(base64String);
        }

        /// <summary>
        /// Converts an URL-safe Base64 encoded data string into a regular Bas64 data string.
        /// </summary>
        /// <param name="base64UrlString">The URL-safe Base64 data string. Can be <c>null</c>.</param>
        /// <returns>A regular Base64 data string that represents the exact same that as <paramref name="base64UrlString"/>. <c>null</c> if <paramref name="base64UrlString"/> is <c>null</c>.</returns>
        /// <exception cref="FormatException"><paramref name="base64UrlString"/> is not a valid URL-safe Base64 encoded data string. The length of the string is invalid.</exception>
        public static string ToRegularBase64String(string base64UrlString) =>
            string.IsNullOrWhiteSpace(base64UrlString) ? null : FromBase64UrlString(new StringBuilder(base64UrlString));
    }
}
