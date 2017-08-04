namespace THNETII.Security.JOSE
{
    /// <summary>
    /// Defines common IANA Media Types used for JOSE.
    /// </summary>
    public static class JoseMediaTypes
    {
        /// <summary>
        /// The shortened IANA Media Type for JSON content.
        /// </summary>
        public const string JsonShortMediaType = "JSON";

        /// <summary>
        /// The shortened IANA Media Type for JOSE content using Compact Serialization.
        /// </summary>
        public const string JoseShortMediaType = "JOSE";

        /// <summary>
        /// The shortened IANA Media Type for JOSE content using JSON Serialization.
        /// </summary>
        public const string JoseJsonShortMediaType = JoseShortMediaType + JsonShortMediaType;

        /// <summary>
        /// The IANA Media Type used by applications to indicate the usage of a
        /// JWS or JWE using the JWS Compact Serialization or the JWE Compact
        /// Serialization.
        /// </summary>
        public const string JoseCompactMediaType = "application/" + JoseShortMediaType;

        /// <summary>
        /// The IANA Media Type used by applications to indicate the usage of a
        /// JWS or JWE using the JWS JSON Serialization or the JWE JSON
        /// Serialization.
        /// </summary>
        public const string JoseJsonMediaType = "application/" + JoseJsonShortMediaType;
    }
}
