using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using THNETII.Common;
using THNETII.Common.Serialization;

namespace THNETII.Security.JOSE
{
    public class JsonOctWebKey : JsonWebKey
    {
        private readonly DuplexConversionTuple<string, byte[]> k = new DuplexConversionTuple<string, byte[]>(Base64UrlEncoder.FromBase64UrlString, Base64UrlEncoder.ToBase64UrlString);

        /// <summary>
        /// The "k" (key value) parameter contains the value of the symmetric (or
        /// other single-valued) key. It is represented as the base64url
        /// encoding of the octet sequence containing the key value.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Member stores the string representation in case non-URI data is deserialized.")]
        [DataMember(Name = "k")]
        public string KeyBase64UrlString
        {
            get => k.RawValue;
            set => k.RawValue = value;
        }

        /// <summary>
        /// Gets or sets the octet sequence containing the key value.
        /// </summary>
        [IgnoreDataMember]
        public byte[] Key
        {
            get => k.ConvertedValue;
            set => k.ConvertedValue = value;
        }

        public JsonOctWebKey() : base(JsonWebKeyType.Oct) { }
    }
}
