using Newtonsoft.Json;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

using THNETII.Common;
using THNETII.TypeConverter;

namespace THNETII.Security.JOSE
{
    /// <summary>
    /// Represents the content protection elements of a JSON Web Signature, containing the protected and unprotected headers and the signature of the payload.
    /// </summary>
    public class JsonWebSignatureFlat
    {
        private readonly DuplexConversionTuple<JsonWebSignatureHeader, string> @protected =
            new DuplexConversionTuple<JsonWebSignatureHeader, string>(@protected =>
            {
                if (@protected is null)
                    return null;
                var jsonString = JsonConvert.SerializeObject(@protected);
                var base64UrlBytes = Encoding.UTF8.GetBytes(jsonString);
                return Base64UrlEncoder.ToBase64UrlString(base64UrlBytes);
            }, base64UrlSring =>
            {
                if (base64UrlSring is null)
                    return null;
                var rawBytes = Base64UrlEncoder.FromBase64UrlString(base64UrlSring);
                if (rawBytes is null)
                    return null;
                var jsonString = Encoding.UTF8.GetString(rawBytes);
                return JsonConvert.DeserializeObject<JsonWebSignatureHeader>(jsonString);
            });
        private readonly DuplexConversionTuple<string, byte[]> signature =
            new DuplexConversionTuple<string, byte[]>(Base64UrlEncoder.FromBase64UrlString, Base64UrlEncoder.ToBase64UrlString);

        /// <summary>
        /// Gets or sets the unprotected header of the Signature. Contains the data that does NOT take part in the integrity protection mechanism of the JWS.
        /// </summary>
        /// <remarks>
        /// The "header" member MUST be present and contain the value JWS
        /// Unprotected Header when the JWS Unprotected Header value is non-
        /// empty; otherwise, it MUST be absent. This value is represented as
        /// an unencoded JSON object, rather than as a string. These Header
        /// Parameter values are not integrity protected.
        /// </remarks>
        [DataMember(Name = "header", EmitDefaultValue = false)]
        public JsonWebSignatureHeader UnprotectedHeader { get; set; }

        /// <summary>
        /// Gets or sets the Base64 URL-safe encoded protected header for this signature element.
        /// </summary>
        /// <remarks>
        /// The "protected" member MUST be present and contain the
        /// URL-safe Base64-encoded JSON representation of the JWS protected
        /// header used together with the <see cref="Signature"/> stored in this instance.
        /// These Header Parameter values are integrity protected.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Member stores the string representation in case non-URI data is deserialized.")]
        [DataMember(Name = "protected", EmitDefaultValue = false)]
        public string ProtectedBase64UrlString
        {
            get => @protected.ConvertedValue;
            set => @protected.ConvertedValue = value;
        }

        /// <summary>
        /// Gets or sets the protected header instance for this signature element.
        /// </summary>
        /// <remarks>
        /// The "protected" member MUST be present and contain the
        /// URL-safe Base64-encoded JSON representation of the JWS protected
        /// header used together with the <see cref="Signature"/> stored in this instance.
        /// These Header Parameter values are integrity protected.
        /// </remarks>
        [IgnoreDataMember]
        public JsonWebSignatureHeader ProtectedHeader
        {
            get => @protected.RawValue;
            set => @protected.RawValue = value;
        }

        /// <summary>
        /// Gets or sets the Base64 URL-safe encoded signature stored in this JWS signature element.
        /// <para>The prarameters specified in <see cref="UnprotectedHeader"/> and <see cref="ProtectedHeader"/> should be used when computing or verifying the value of this property.</para>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Member stores the string representation in case non-URI data is deserialized.")]
        [DataMember(Name = "signature", IsRequired = true)]
        public string SignatureBase64UrlString
        {
            get => signature.RawValue;
            set => signature.RawValue = value;
        }

        /// <summary>
        /// Gets or sets signature stored in this JWS signature element.
        /// <para>The prarameters specified in <see cref="UnprotectedHeader"/> and <see cref="ProtectedHeader"/> should be used when computing or verifying the value of this property.</para>
        /// </summary>
        [IgnoreDataMember]
        public byte[] Signature
        {
            get => signature.ConvertedValue;
            set => signature.ConvertedValue = value;
        }

        /// <summary>
        /// Gets the signature input using the specified payload.
        /// </summary>
        /// <returns>The Base64 URL-safe representations of the protected header and the payload separated by a period (<c>'.'</c>) character.</returns>
        /// <remarks>Cryptographic Signature algorithms in the .NET Standard typically require a byte array as signing input. Use the <see cref="Encoding.GetBytes(string)"/> method of the static <see cref="Encoding.ASCII"/> instance in the <see cref="Encoding"/> class to get a byte array signing input.</remarks>
        public string GetSignatureInput(byte[] payload)
        {
            return string.Join(".", ProtectedBase64UrlString ?? string.Empty, Base64UrlEncoder.ToBase64UrlString(payload) ?? string.Empty);
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
