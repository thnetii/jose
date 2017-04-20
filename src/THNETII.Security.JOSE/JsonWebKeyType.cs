using System.Runtime.Serialization;

namespace THNETII.Security.JOSE
{
    public enum JsonWebKeyType
    {
        Unknown,
        /// <summary>
        /// JWKs can represent Elliptic Curve [<a href="https://tools.ietf.org/html/rfc7518.html#ref-DSS">DSS</a>] keys.  In this case, the
        /// <c>"kty"</c> member value is <c>"EC"</c>.
        /// </summary>
        [EnumMember(Value = "EC")]
        Ec,
        /// <summary>
        /// JWKs can represent RSA [<a href="https://tools.ietf.org/html/rfc3447">RFC3447</a>] keys.  In this case, the <c>"kty"</c>
        /// member value is <c>"RSA"</c>.  The semantics of the parameters defined for the JSON Web Key
        /// are the same as those defined in Sections <a href="https://tools.ietf.org/html/rfc3447#section-3.1">3.1</a> and
        /// <a href="https://tools.ietf.org/html/rfc3447#section-3.2">3.2</a> of <a href="https://tools.ietf.org/html/rfc3447">RFC 3447</a>.
        /// </summary>
        [EnumMember(Value = "RSA")]
        Rsa,
    }
}
