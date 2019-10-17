using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

using THNETII.Common;
using THNETII.TypeConverter;

namespace THNETII.Security.JOSE
{
    /// <summary>
    /// Represents a JOSE header used in a JSON Web Signature.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For a JWS, the members of the JSON object(s) representing the JOSE
    /// Header describe the digital signature or MAC applied to the JWS
    /// Protected Header and the JWS Payload and optionally additional
    /// properties of the JWS.  The Header Parameter names within the JOSE
    /// Header MUST be unique; JWS parsers MUST either reject JWSs with
    /// duplicate Header Parameter names or use a JSON parser that returns
    /// only the lexically last duplicate member name, as specified in
    /// Section 15.12 ("The JSON Object") of the 
    /// <a href="http://www.ecma-international.org/ecma-262/5.1/ECMA-262.pdf">ECMAScript 5.1</a>
    /// specification.
    /// </para>
    /// <para>
    /// The JWS JOSE header is specified by 
    /// <a href="https://tools.ietf.org/html/rfc7515#section-4">Section 4. JOSE Header</a> in the
    /// <a href="https://tools.ietf.org/html/rfc7515">RFC7515 - JSON Web Signature (JWS)</a> specification.
    /// </para>
    /// </remarks>
    public class JsonWebSignatureHeader
    {
        private const string applicationMediaTypePrefix = "application/";
        private readonly DuplexConversionTuple<string, JsonWebAlgorithm> alg =
            new DuplexConversionTuple<string, JsonWebAlgorithm>(EnumStringConverter.ParseOrDefault<JsonWebAlgorithm>, EnumStringConverter.ToString);
        private readonly DuplexConversionTuple<string, Uri> jku =
            new DuplexConversionTuple<string, Uri>(UriStringConverter.ParseOrDefault, UriStringConverter.ToString);
        private DuplexConversionTuple<string, MediaTypeHeaderValue> typ =
            new DuplexConversionTuple<string, MediaTypeHeaderValue>(StringToMediaTypeOrDefault, MediaTypeToString);
        private DuplexConversionTuple<string, MediaTypeHeaderValue> cty =
            new DuplexConversionTuple<string, MediaTypeHeaderValue>(StringToMediaTypeOrDefault, MediaTypeToString);

        /// <summary>
        /// Gets or sets the JSON Web Algorithm name that is used.
        /// <para>
        /// A list of defined "alg" values for this use can be found in the IANA
        /// "JSON Web Signature and Encryption Algorithms" registry established
        /// by <a href="https://tools.ietf.org/html/rfc7518" title="JSON Web Algorithms">RFC 7518</a>;
        /// the initial contents of this registry are the values defined in 
        /// <a href="https://tools.ietf.org/html/rfc7518#section-3.1" title="&quot;alg&quot; (Algorithm) Header Parameter Values for JWS">Section 3.1</a>
        /// of <a href="https://tools.ietf.org/html/rfc7518" title="JSON Web Algorithms">RFC 7518</a>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Settings this property will also set the value of the <see cref="Algorithm"/>
        /// property to the appropiate value of the <see cref="JsonWebAlgorithm"/> enumeration.
        /// </para>
        /// <para>
        /// The "alg" (algorithm) Header Parameter identifies the cryptographic
        /// algorithm used to secure the JWS. The JWS Signature value is not
        /// valid if the "alg" value does not represent a supported algorithm or
        /// if there is not a key for use with that algorithm associated with the
        /// party that digitally signed or MACed the content. "alg" values
        /// should either be registered in the IANA "JSON Web Signature and
        /// Encryption Algorithms" registry established by
        /// <a href="https://tools.ietf.org/html/rfc7518" title="JSON Web Algorithms">RFC 7518</a>
        /// or be a value
        /// that contains a Collision-Resistant Name.  The "alg" value is a case-
        /// sensitive ASCII string containing a string or URI value. This Header
        /// Parameter MUST be present and MUST be understood and processed by
        /// implementations.
        /// </para>
        /// </remarks>
        [DataMember(Name = "alg", IsRequired = true)]
        public string AlgorithmName
        {
            get => alg.RawValue;
            set => alg.RawValue = value;
        }

        /// <summary>Gets or set the JSON Web Algorithm that is used.</summary>
        /// <remarks>
        /// The "alg" (algorithm) Header Parameter identifies the cryptographic
        /// algorithm used to secure the JWS. The JWS Signature value is not
        /// valid if the "alg" value does not represent a supported algorithm or
        /// if there is not a key for use with that algorithm associated with the
        /// party that digitally signed or MACed the content. "alg" values
        /// should either be registered in the IANA "JSON Web Signature and
        /// Encryption Algorithms" registry established by [JWA] or be a value
        /// that contains a Collision-Resistant Name.  The "alg" value is a case-
        /// sensitive ASCII string containing a StringOrURI value. This Header
        /// Parameter MUST be present and MUST be understood and processed by
        /// implementations.
        /// </remarks>
        [IgnoreDataMember]
        public JsonWebAlgorithm Algorithm
        {
            get => alg.ConvertedValue;
            set => alg.ConvertedValue = value;
        }

        /// <remarks>
        /// The "jku" (JWK Set URL) Header Parameter is a URI [RFC3986] that
        /// refers to a resource for a set of JSON-encoded public keys, one of
        /// which corresponds to the key used to digitally sign the JWS. The
        /// keys MUST be encoded as a JWK Set. The protocol used to
        /// acquire the resource MUST provide integrity protection; an HTTP GET
        /// request to retrieve the JWK Set MUST use Transport Layer Security
        /// (TLS) [RFC2818] [RFC5246]; and the identity of the server MUST be
        /// validated, as per Section 6 of RFC 6125 [RFC6125]. Also, see
        /// Section 8 of RFC7515 on TLS requirements. Use of this Header Parameter is
        /// OPTIONAL.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Member stores the string representation in case non-URI data is deserialized.")]
        [DataMember(Name = "jku", IsRequired = false, EmitDefaultValue = false)]
        public string JwkSetUrlString
        {
            get => jku.RawValue;
            set => jku.RawValue = value;
        }

        /// <remarks>
        /// The "jku" (JWK Set URL) Header Parameter is a URI [RFC3986] that
        /// refers to a resource for a set of JSON-encoded public keys, one of
        /// which corresponds to the key used to digitally sign the JWS. The
        /// keys MUST be encoded as a JWK Set. The protocol used to
        /// acquire the resource MUST provide integrity protection; an HTTP GET
        /// request to retrieve the JWK Set MUST use Transport Layer Security
        /// (TLS) [RFC2818] [RFC5246]; and the identity of the server MUST be
        /// validated, as per Section 6 of RFC 6125 [RFC6125]. Also, see
        /// Section 8 of RFC7515 on TLS requirements. Use of this Header Parameter is
        /// OPTIONAL.
        /// </remarks>
        [IgnoreDataMember]
        public Uri JwkSetUri
        {
            get => jku.ConvertedValue;
            set => jku.ConvertedValue = value;
        }

        /// <remarks>
        /// The "jwk" (JSON Web Key) Header Parameter is the public key that
        /// corresponds to the key used to digitally sign the JWS.This key is
        /// represented as a JSON Web Key [JWK]. Use of this Header Parameter is
        /// OPTIONAL.
        /// </remarks>
        [DataMember(Name = "jwk", IsRequired = false, EmitDefaultValue = false)]
        public JsonWebKey JsonWebKey { get; set; }

        /// <remarks>
        /// The "kid" (key ID) Header Parameter is a hint indicating which key
        /// was used to secure the JWS. This parameter allows originators to
        /// explicitly signal a change of key to recipients. The structure of
        /// the "kid" value is unspecified. Its value MUST be a case-sensitive
        /// string. Use of this Header Parameter is OPTIONAL.
        /// <para>
        /// When used with a JWK, the "kid" value is used to match a JWK "kid"
        /// parameter value.
        /// </para>
        /// </remarks>
        [DataMember(Name = "kid", IsRequired = false, EmitDefaultValue = false)]
        public string KeyId { get; set; }

        [DataMember(Name = "typ", IsRequired = false, EmitDefaultValue = false)]
        public string SignatureMediaTypeString
        {
            get => typ.RawValue;
            set => typ.RawValue = value;
        }

        [IgnoreDataMember]
        public MediaTypeHeaderValue SignatureMediaType
        {
            get => typ.ConvertedValue;
            set => typ.ConvertedValue = value;
        }

        [DataMember(Name = "cty", IsRequired = false, EmitDefaultValue = false)]
        public string ContentTypeString
        {
            get => cty.RawValue;
            set => cty.RawValue = value;
        }

        [IgnoreDataMember]
        public MediaTypeHeaderValue ContentType
        {
            get => cty.ConvertedValue;
            set => cty.ConvertedValue = value;
        }

        [DataMember(Name = "crit", IsRequired = false, EmitDefaultValue = false)]
        public string[] CrtiticalHeaders { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

        private static MediaTypeHeaderValue StringToMediaTypeOrDefault(string mediaTypeString)
        {
            if (mediaTypeString is null)
                return null;
            else if (MediaTypeHeaderValue.TryParse(mediaTypeString, out var mediaType))
            {
                if (mediaType.MediaType.StartsWith(applicationMediaTypePrefix, StringComparison.OrdinalIgnoreCase) && mediaType.Parameters.Count == 0)
                    return MediaTypeHeaderValue.TryParse(applicationMediaTypePrefix + mediaTypeString, out mediaType) ? mediaType : null;
                return mediaType;
            }
            return null;
        }

        private static string MediaTypeToString(MediaTypeHeaderValue mediaType)
        {
            if (mediaType is null)
                return null;
            else if (mediaType.MediaType.StartsWith(applicationMediaTypePrefix, StringComparison.OrdinalIgnoreCase) && mediaType.Parameters.Count == 0)
                return mediaType.ToString().Substring(applicationMediaTypePrefix.Length);
            else
                return mediaType.ToString();
        }

#if DEBUG
        public override string ToString() => JsonConvert.SerializeObject(this);
#endif // DEBUG
    }
}
