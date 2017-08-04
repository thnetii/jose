using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using THNETII.Common;

namespace THNETII.Security.JOSE
{
    /// <summary>
    /// JSON Web Signature (JWS) represents content secured with digital
    /// signatures or Message Authentication Codes (MACs) using JSON-based
    /// data structures.
    /// </summary>
    /// <remarks>JWS is specified by <a href="https://tools.ietf.org/html/rfc7515">RFC 7515 - JSON Web Signature (JWS)</a>.</remarks>
    public class JsonWebSignature : JsonWebSignatureFlat
    {
        private readonly DuplexConversionTuple<string, byte[]> payload =
            new DuplexConversionTuple<string, byte[]>(Base64UrlEncoder.FromBase64UrlString, Base64UrlEncoder.ToBase64UrlString);

        /// <summary>
        /// Gets or sets the JWS Payload Base64 URL-safe encoded string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The "payload" member MUST be present and contain the
        /// JWS Payload as a Base64 URL-safe encoded string.
        /// </para>
        /// <para>Setting this property will also affect the value of the <see cref="Payload"/> property.</para>
        /// <para>
        /// The value of this property is part of the signing input when computing the Signature included in the JWS.
        /// </para>
        /// </remarks>
        [DataMember(Name = "payload", IsRequired = true)]
        public string PayloadBase64UrlString
        {
            get => payload.RawValue;
            set => payload.RawValue = value;
        }

        /// <summary>
        /// Gets or sets the JWS Payload Raw Bytes.
        /// </summary>
        [IgnoreDataMember]
        public byte[] Payload
        {
            get => payload.ConvertedValue;
            set => payload.ConvertedValue = value;
        }

        /// <summary>
        /// Gets or sets the collection of signatures, each element in the collection containing the signature value with its individual JOSE header.
        /// <para>Use only if multiple signatures are used. Only supported for general JWS serialization representation.</para>
        /// </summary>
        /// <value><c>null</c> by default; otherwise, it should contain a non-empty collection.</value>
        /// <remarks>Setting this property to a non-<c>null</c> value changes the JWS Serialization from flat to general JWS serialization mode.</remarks>
        [DataMember(Name = "signatures", EmitDefaultValue = false)]
        public ICollection<JsonWebSignatureFlat> Signatures { get; set; }

        /// <summary>
        /// The JWS Compact Serialization represents digitally signed or MACed
        /// content as a compact, URL-safe string.
        /// </summary>
        /// <returns>A Base64 URL-safe encoded string, with the <see cref="JsonWebSignatureFlat.ProtectedBase64UrlString"/>, the <see cref="PayloadBase64UrlString"/> and the <see cref="JsonWebSignatureFlat.SignatureBase64UrlString"/> each separated with a period ('.') character.</returns>
        /// <remarks>
        /// Only one signature/MAC is supported by the JWS Compact Serialization
        /// and it provides no syntax to represent a JWS Unprotected Header
        /// value.
        /// </remarks>
        public string ToCompactString()
        {
            if (Signatures != null || Signature.Length != 0)
                throw new InvalidOperationException();

            return string.Join(".",
                ProtectedBase64UrlString ?? string.Empty,
                PayloadBase64UrlString ?? string.Empty,
                SignatureBase64UrlString ?? string.Empty
                );
        }

        /// <summary>
        /// Gets the signature input for a flattened single signature JWS.
        /// </summary>
        /// <returns>The Base64 URL-safe representations of the protected header and the payload separated by a period (<c>'.'</c>) character.</returns>
        /// <remarks>Cryptographic Signature algorithms in the .NET Standard typically require a byte array as signing input. Use the <see cref="Encoding.GetBytes(string)"/> method of the static <see cref="Encoding.ASCII"/> instance in the <see cref="Encoding"/> class to get a byte array signing input.</remarks>
        public string GetSignatureInput() => GetSignatureInput(Payload);

#if DEBUG
        /// <summary>
        /// Returns the JSON representation of this instance.
        /// </summary>
        /// <returns>A JSON string of the current instance.</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
#endif // DEBUG
    }
}
