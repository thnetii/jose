using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using THNETII.Common;

namespace THNETII.Security.JOSE
{
    /// <summary>
    /// A JWK is a JSON object that represents a cryptographic key.  The
    /// members of the object represent properties of the key, including its
    /// value.
    /// <para>Follows the <a href="https://tools.ietf.org/html/rfc7517">JSON Web Key (JWK) Specification [RFC7517]</a></para>
    /// </summary>
    /// <remarks>
    /// In addition to the common parameters, each JWK will have members that
    /// are key type specific. These members represent the parameters of the
    /// key. <a href="https://tools.ietf.org/html/rfc7518#section-6">Section 6</a> of the <a href="https://tools.ietf.org/html/rfc7518">JSON Web Algorithms (JWA) specification [RFC7518]</a>
    /// defines multiple kinds of cryptographic keys and their associated
    /// members.
    /// </remarks>
    [DataContract]
    public class JsonWebKey
    {
        private readonly DuplexConversionTuple<string, JsonWebKeyType> kty =
            new DuplexConversionTuple<string, JsonWebKeyType>(ConvertStringToKty, ConvertKtyToString);

        /// <summary>
        /// The "kty" (key type) parameter identifies the cryptographic algorithm
        /// family used with the key, such as "RSA" or "EC". "kty" values should
        /// either be registered in the IANA "JSON Web Key Types" registry
        /// established by [<a href="https://tools.ietf.org/html/rfc7518">JWA</a>] or be a value that contains a Collision-Resistant 
        /// Name. The "kty" value is a case-sensitive string. This
        /// member MUST be present in a JWK.
        /// </summary>
        [DataMember(Name = "kty")]
        public virtual string KeyTypeString
        {
            get => kty.RawValue;
            set => kty.RawValue = value;
        }

        /// <summary>
        /// Gets an enumeration value for an IANA "JSON Web Key Types"
        /// declared JSON Web Key Type. If the value is <see cref="JsonWebKeyType.Unknown"/>
        /// <see cref="KeyTypeString"/> will contain the non-standard key type identifier that is used.
        /// </summary>
        [IgnoreDataMember]
        public virtual JsonWebKeyType KeyType
        {
            get => kty.ConvertedValue;
            set => kty.ConvertedValue = value;
        }

        /// <summary>
        /// Additional members can be present in the JWK. Member
        /// names used for representing key parameters for different keys types
        /// need not be distinct. Any new member name should either be
        /// registered in the IANA "JSON Web Key Parameters" registry established
        /// by <a href="https://tools.ietf.org/html/rfc7517#section-8.1">Section 8.1 in RFC7517</a>
        /// or be a value that contains a Collision-Resistant Name.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

        private static JsonWebKeyType ConvertStringToKty(string rawKty)
        {
            if (Enum.TryParse(rawKty, ignoreCase: true, result: out JsonWebKeyType kty))
                return kty;
            else
                return JsonWebKeyType.Unknown;
        }

        private static string ConvertKtyToString(JsonWebKeyType kty)
        {
            switch (kty)
            {
                case JsonWebKeyType.Ec: return "EC";
                case JsonWebKeyType.Rsa: return "RSA";
                default: return null;
            }
        }
    }
}
