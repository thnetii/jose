using System.Runtime.Serialization;

using THNETII.Common;
using THNETII.TypeConverter;

namespace THNETII.Security.JOSE
{
    /// <summary>
    /// A JSON Web Key (JWK) is a JavaScript Object Notation (JSON) data
    /// structure that represents a cryptographic key.
    /// </summary>
    /// <remarks>The JSON Web Key (JWK) is specified in <a href="https://tools.ietf.org/html/rfc7517">RFC 7517</a>.</remarks>
    [DataContract]
    public class JsonRsaWebKey : JsonWebKey
    {
        private readonly DuplexConversionTuple<string, byte[]> n = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> e = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> d = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> p = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> q = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> dp = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> dq = CreateBase64UrlDuplexConversionTuple();
        private readonly DuplexConversionTuple<string, byte[]> qi = CreateBase64UrlDuplexConversionTuple();

        /// <summary>
        /// The RSA modulus, expressed as a byte array to express very large integer values.
        /// </summary>
        [IgnoreDataMember]
        public byte[] N
        {
            get => n.ConvertedValue;
            set => n.ConvertedValue = value;
        }

        /// <summary>
        /// The RSA public exponent, expressed as a byte array to express very large integer values.
        /// </summary>
        [IgnoreDataMember]
        public byte[] E
        {
            get => e.ConvertedValue;
            set => e.ConvertedValue = value;
        }

        /// <summary>
        /// The RSA private exponent, expressed as a byte array to express very large integer values.
        /// </summary>
        [IgnoreDataMember]
        public byte[] D
        {
            get => d.ConvertedValue;
            set => d.ConvertedValue = value;
        }

        /// <summary>
        /// The first factor.
        /// </summary>
        [IgnoreDataMember]
        public byte[] P
        {
            get => p.ConvertedValue;
            set => p.ConvertedValue = value;
        }

        /// <summary>
        /// The second factor.
        /// </summary>
        [IgnoreDataMember]
        public byte[] Q
        {
            get => q.ConvertedValue;
            set => q.ConvertedValue = value;
        }

        /// <summary>
        /// The first factor's CRT exponent
        /// </summary>
        [IgnoreDataMember]
        public byte[] DP
        {
            get => dp.ConvertedValue;
            set => dp.ConvertedValue = value;
        }

        /// <summary>
        /// The second factor's CRT exponent
        /// </summary>
        [IgnoreDataMember]
        public byte[] DQ
        {
            get => dq.ConvertedValue;
            set => dq.ConvertedValue = value;
        }

        /// <summary>
        /// The (first) CRT coefficient
        /// </summary>
        [IgnoreDataMember]
        public byte[] QI
        {
            get => qi.ConvertedValue;
            set => qi.ConvertedValue = value;
        }

        /// <summary>
        /// The RSA modulus, expressed as a Base64-URL-safe-encoded byte string.
        /// </summary>
        [DataMember(Name = "n")]
        public string NBase64String
        {
            get => n.RawValue;
            set => n.RawValue = value;
        }

        /// <summary>
        /// The RSA public exponent, expressed as a Base64-URL-safe-encoded byte string.
        /// </summary>
        [DataMember(Name = "e")]
        public string EBase64String
        {
            get => e.RawValue;
            set => e.RawValue = value;
        }

        /// <summary>
        /// The RSA private exponent, expressed as a Base64-URL-safe-encoded byte string.
        /// </summary>
        [DataMember(Name = "d", IsRequired = false, EmitDefaultValue = false)]
        public string DBase64String
        {
            get => d.RawValue;
            set => d.RawValue = value;
        }

        /// <summary>
        /// The first factor, expressed as a Base64-URL-safe-encoded byte string.
        /// </summary>
        [DataMember(Name = "p", IsRequired = false, EmitDefaultValue = false)]
        public string PBase64String
        {
            get => p.RawValue;
            set => p.RawValue = value;
        }

        /// <summary>
        /// The second factor, expressed as a Base64-URL-safe-encoded byte string.
        /// </summary>
        [DataMember(Name = "q", IsRequired = false, EmitDefaultValue = false)]
        public string QBase64String
        {
            get => q.RawValue;
            set => q.RawValue = value;
        }

        /// <summary>
        /// The first factor's CRT exponent, expressed as a Base64-URL-safe-encoded byte string.
        /// </summary>
        [DataMember(Name = "dp", IsRequired = false, EmitDefaultValue = false)]
        public string DPBase64String
        {
            get => dp.RawValue;
            set => dp.RawValue = value;
        }

        /// <summary>
        /// The second factor's CRT exponent, expressed as a Base64-URL-safe-encoded byte string.
        /// </summary>
        [DataMember(Name = "dq", IsRequired = false, EmitDefaultValue = false)]
        public string DQBase64String
        {
            get => dq.RawValue;
            set => dq.RawValue = value;
        }

        /// <summary>
        /// The first CRT coefficient, expressed as a Base64-URL-safe-encoded byte string.
        /// </summary>
        [DataMember(Name = "qi", IsRequired = false, EmitDefaultValue = false)]
        public string QIBase64String
        {
            get => qi.RawValue;
            set => qi.RawValue = value;
        }

        private static DuplexConversionTuple<string, byte[]> CreateBase64UrlDuplexConversionTuple()
        {
            return new DuplexConversionTuple<string, byte[]>(
                Base64UrlEncoder.FromBase64UrlString,
                Base64UrlEncoder.ToBase64UrlString
                );
        }

        /// <summary>
        /// Creates a new RSA JWK.
        /// </summary>
        public JsonRsaWebKey() : base(JsonWebKeyType.Rsa) { }
    }
}
