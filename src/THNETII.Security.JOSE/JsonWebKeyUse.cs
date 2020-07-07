using System.Runtime.Serialization;

namespace THNETII.Security.JOSE
{
    public enum JsonWebKeyUse
    {
        Unknown = 0,
        [EnumMember(Value = "enc")]
        Encryption,
        [EnumMember(Value = "sig")]
        Signature
    }
}
