using System.Runtime.Serialization;

namespace THNETII.Security.JOSE
{
    public enum JsonWebAlgorithm
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
        None,
        /// <summary>
        /// RSAES-PKCS1-v1_5
        /// </summary>
        [EnumMember]
        RSA1_5,
        /// <summary>
        /// RSAES OAEP using default parameters
        /// </summary>
        [EnumMember(Value = "RSA-OAEP")]
        RSA_OAEP,
        /// <summary>
        /// RSAES OAEP using SHA-256 and MGF1 with
        /// SHA-256
        /// </summary>
        [EnumMember(Value = "RSA-OAEP-256")]
        RSA_OAEP_256,
        /// <summary>
        /// AES Key Wrap using 128-bit key
        /// </summary>
        [EnumMember]
        A128KW,
        /// <summary>
        /// AES Key Wrap using 192-bit key
        /// </summary>
        [EnumMember]
        A192KW,
        /// <summary>
        /// AES Key Wrap using 256-bit key
        /// </summary>
        [EnumMember]
        A256KW,
        /// <summary>
        /// Direct use of a shared symmetric key
        /// </summary>
        [EnumMember(Value = "dir")]
        Direct,
        /// <summary>
        /// ECDH-ES using Concat KDF
        /// </summary>
        [EnumMember(Value = "ECDH-ES")]
        ECDH_ES,
        /// <summary>
        /// ECDH-ES using Concat KDF and <see cref="A128KW"/>
        /// </summary>
        [EnumMember(Value = "ECDH-ES+A128KW")]
        ECDH_ES_A128KW,
        /// <summary>
        /// ECDH-ES using Concat KDF and <see cref="A192KW"/>
        /// </summary>
        [EnumMember(Value = "ECDH-ES+A192KW")]
        ECDH_ES_A192KW,
        /// <summary>
        /// ECDH-ES using Concat KDF and <see cref="A256KW"/>
        /// </summary>
        [EnumMember(Value = "ECDH-ES+A256KW")]
        ECDH_ES_A256KW,
        /// <summary>
        /// Key wrapping with AES GCM using 128-bit key
        /// </summary>
        [EnumMember]
        A128GCMKW,
        /// <summary>
        /// Key wrapping with AES GCM using 192-bit key
        /// </summary>
        [EnumMember]
        A192GCMKW,
        /// <summary>
        /// Key wrapping with AES GCM using 256-bit key
        /// </summary>
        [EnumMember]
        A256GCMKW,
        /// <summary>
        /// PBES2 with HMAC SHA-256 and <see cref="A128KW"/>
        /// </summary>
        [EnumMember(Value = "PBES2-HS256+A128KW")]
        PBES2_HS256_A128KW,
        /// <summary>
        /// PBES2 with HMAC SHA-384 and <see cref="A192KW"/>
        /// </summary>
        [EnumMember(Value = "PBES2-HS384+A192KW")]
        PBES2_HS384_A192KW,
        /// <summary>
        /// PBES2 with HMAC SHA-512 and <see cref="A256KW"/>
        /// </summary>
        [EnumMember(Value = "PBES2-HS512+A256KW")]
        PBES2_HS512_A256KW,
        /// <summary>
        /// AES_128_CBC_HMAC_SHA_256 authenticated
        /// encryption algorithm
        /// </summary>
        [EnumMember(Value = "A128CBC-HS256")]
        A128CBC_HS256,
        /// <summary>
        /// AES_192_CBC_HMAC_SHA_384 authenticated
        /// encryption algorithm
        /// </summary>
        [EnumMember(Value = "A192CBC-HS384")]
        A192CBC_HS384,
        /// <summary>
        /// AES_256_CBC_HMAC_SHA_512 authenticated
        /// encryption algorithm
        /// </summary>
        [EnumMember(Value = "A256CBC-HS512")]
        A256CBC_HS512,
        /// <summary>
        /// AES GCM using 128-bit key
        /// </summary>
        [EnumMember]
        A128GCM,
        /// <summary>
        /// AES GCM using 192-bit key
        /// </summary>
        [EnumMember]
        A192GCM,
        /// <summary>
        /// AES GCM using 256-bit key
        /// </summary>
        [EnumMember]
        A256GCM,
    }
}
