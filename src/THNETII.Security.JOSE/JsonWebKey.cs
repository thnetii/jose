using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using THNETII.Common;
using THNETII.Common.Serialization;

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
    [DataContract, JsonConverter(typeof(JsonWebKeyConverter))]
    public class JsonWebKey
    {
        /// <summary>
        /// Gets the JSON property key of a serialized JSON Web Key instance.
        /// </summary>
        public const string KeyTypeJsonPropertyName = "kty";

        private readonly Tuple<string, JsonWebKeyType> lockKty;
        private readonly DuplexConversionTuple<string, JsonWebKeyType> kty =
            new DuplexConversionTuple<string, JsonWebKeyType>(EnumStringConverter.ParseOrDefault<JsonWebKeyType>, EnumStringConverter.ToString);

        /// <summary>
        /// The "kty" (key type) parameter identifies the cryptographic algorithm
        /// family used with the key, such as "RSA" or "EC". "kty" values should
        /// either be registered in the IANA "JSON Web Key Types" registry
        /// established by [<a href="https://tools.ietf.org/html/rfc7518">JWA</a>] or be a value that contains a Collision-Resistant 
        /// Name. The "kty" value is a case-sensitive string. This
        /// member MUST be present in a JWK.
        /// </summary>
        /// <exception cref="InvalidOperationException">This instance is locked to a specific key type and its type must not be changed.</exception>
        [DataMember(Name = KeyTypeJsonPropertyName)]
        public virtual string KeyTypeString
        {
            get => kty.RawValue;
            set => kty.RawValue = AssertKtyStringValue(value);
        }

        private string AssertKtyStringValue(string value)
        {
            if (lockKty == null || lockKty.Item1.Equals(value, StringComparison.OrdinalIgnoreCase))
                return value;
            throw new InvalidOperationException($"{nameof(value)} must be \"{lockKty.Item1}\".");
        }

        /// <summary>
        /// Gets an enumeration value for an IANA "JSON Web Key Types"
        /// declared JSON Web Key Type. If the value is <see cref="JsonWebKeyType.Unknown"/>
        /// <see cref="KeyTypeString"/> will contain the non-standard key type identifier that is used.
        /// </summary>
        /// <exception cref="InvalidOperationException">This instance is locked to a specific key type and its type must not be changed.</exception>
        [IgnoreDataMember]
        public virtual JsonWebKeyType KeyType
        {
            get => kty.ConvertedValue;
            set => kty.ConvertedValue = AssertKtyValue(value);
        }

        private JsonWebKeyType AssertKtyValue(JsonWebKeyType value)
        {
            if (lockKty == null || lockKty.Item2 == value)
                return value;
            throw new InvalidOperationException($"{nameof(value)} must be the {lockKty.Item2} value of the {nameof(JsonWebKeyType)} enumeration.");
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

        /// <summary>
        /// Creates a new generic JSON Web Key instance.
        /// </summary>
        public JsonWebKey() { }

        /// <summary>
        /// Creates a new JSON Web Key instance whose key type is locked to the specified type.
        /// </summary>
        /// <param name="lockKty">The key type to which the new instance is locked.</param>
        /// <remarks>This constructor should be called from all constructors of inheriting types to ensure that the Key type cannot be changed once the instance is created.</remarks>
        protected JsonWebKey(JsonWebKeyType lockKty) : this()
        {
            KeyType = lockKty;
            this.lockKty = Tuple.Create(EnumStringConverter.ToString<JsonWebKeyType>(lockKty), lockKty);
        }

#if DEBUG
        /// <summary>
        /// Returns the JSON representation of this instance.
        /// </summary>
        /// <returns>A JSON string of the current instance.</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
#endif // DEBUG
    }
}
