using System.Runtime.Serialization;

namespace THNETII.Security.JOSE
{
    /// <summary>
    /// Defines the set of <c>"alg"</c> (algorithm) Header Parameter
    /// values defined by this specification for use with JWS.
    /// <para>Specified in <a href="https://tools.ietf.org/html/rfc7518#section-3.1">RFC 7518 - JSON Web Algorithms, Section 3.1: "alg" (Algorithm) Header Parameter Values for JWS</a>.</para>
    /// </summary>
    public enum JsonWebSignatureAlgorithm
    {
        Unknown = 0,
        /// <summary>HMAC using SHA-256</summary>
        [EnumMember]
        HS256,
        /// <summary>HMAC using SHA-384</summary>
        [EnumMember]
        HS384,
        /// <summary>HMAC using SHA-512</summary>
        [EnumMember]
        HS512,
        /// <summary>
        /// RSASSA-PKCS1-v1_5 using
        /// SHA-256
        /// </summary>
        [EnumMember]
        RS256,
        /// <summary>
        /// RSASSA-PKCS1-v1_5 using
        /// SHA-384
        /// </summary>
        [EnumMember]
        RS384,
        /// <summary>
        /// RSASSA-PKCS1-v1_5 using
        /// SHA-512
        /// </summary>
        [EnumMember]
        RS512,
        /// <summary>ECDSA using P-256 and SHA-256</summary>
        [EnumMember]
        ES256,
        /// <summary>ECDSA using P-384 and SHA-384</summary>
        [EnumMember]
        ES384,
        /// <summary>ECDSA using P-521 and SHA-512</summary>
        [EnumMember]
        ES512,
        /// <summary>
        /// RSASSA-PSS using SHA-256 and
        /// MGF1 with SHA-256
        /// </summary>
        [EnumMember]
        PS256,
        /// <summary>
        /// RSASSA-PSS using SHA-384 and
        /// MGF1 with SHA-384
        /// </summary>
        [EnumMember]
        PS384,
        /// <summary>
        /// RSASSA-PSS using SHA-512 and
        /// MGF1 with SHA-512
        /// </summary>
        [EnumMember]
        PS512,
        /// <summary>
        /// No digital signature or MAC
        /// performed
        /// </summary>
        [EnumMember(Value = "none")]
        None
    }
}
