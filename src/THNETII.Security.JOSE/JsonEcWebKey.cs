using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;

using THNETII.Common;
using THNETII.TypeConverter;

namespace THNETII.Security.JOSE
{
    public class JsonEcWebKey : JsonWebKey
    {
        private readonly DuplexConversionTuple<string, ECCurve> crv = new DuplexConversionTuple<string, ECCurve>(StringToECCurve, ECCurveToString);
        private readonly DuplexConversionTuple<string, byte[]> x = new DuplexConversionTuple<string, byte[]>(Base64UrlEncoder.FromBase64UrlString, Base64UrlEncoder.ToBase64UrlString);
        private readonly DuplexConversionTuple<string, byte[]> y = new DuplexConversionTuple<string, byte[]>(Base64UrlEncoder.FromBase64UrlString, Base64UrlEncoder.ToBase64UrlString);
        private readonly DuplexConversionTuple<string, byte[]> d = new DuplexConversionTuple<string, byte[]>(Base64UrlEncoder.FromBase64UrlString, Base64UrlEncoder.ToBase64UrlString);

        [DataMember(Name = "crv")]
        public string CurveIdentifierString
        {
            get => crv.RawValue;
            set => crv.RawValue = value;
        }

        [IgnoreDataMember]
        public ECCurve Curve
        {
            get => crv.ConvertedValue;
            set => crv.ConvertedValue = value;
        }

        [DataMember(Name = "x")]
        public string XBase64String
        {
            get => x.RawValue;
            set => x.RawValue = value;
        }

        [DataMember(Name = "y")]
        public string YBase64String
        {
            get => y.RawValue;
            set => y.RawValue = value;
        }

        [IgnoreDataMember]
        public ECPoint Q
        {
            get => new ECPoint() { X = x.ConvertedValue, Y = y.ConvertedValue };
            set
            {
                x.ConvertedValue = value.X;
                y.ConvertedValue = value.Y;
            }
        }

        [DataMember(Name = "d")]
        public string DBase64String
        {
            get => d.RawValue;
            set => d.RawValue = value;
        }

        [IgnoreDataMember]
        public byte[] D
        {
            get => d.ConvertedValue;
            set => d.ConvertedValue = value;
        }

        public JsonEcWebKey() : base(JsonWebKeyType.Ec) { }

        private static ECCurve StringToECCurve(string crv)
        {
            switch (crv)
            {
                case "P-256": return ECCurve.NamedCurves.nistP256;
                case "P-384": return ECCurve.NamedCurves.nistP384;
                case "P-521": return ECCurve.NamedCurves.nistP521;
                case null: throw new ArgumentNullException(nameof(crv));
                default:
                    throw new ArgumentException($"Unknown cryptographic curve: \"{crv}\". Refer to https://tools.ietf.org/html/rfc7518#section-6.2.1.1 for valid values.", nameof(crv));
            }
        }

        private static string ECCurveToString(ECCurve curve)
        {
            if (!curve.IsNamed)
                throw new ArgumentException("Cryptographic curve must be named.", nameof(curve));
            var curveName = curve.Oid.FriendlyName.ThrowIfNullOrWhiteSpace(string.Join(".", nameof(curve), nameof(curve.Oid), nameof(curve.Oid.FriendlyName)));
            if (curveName == ECCurve.NamedCurves.nistP256.Oid.FriendlyName ||
                string.Equals(curveName, "ECDSA_P256", StringComparison.OrdinalIgnoreCase))
                return "P-256";
            else if (curveName == ECCurve.NamedCurves.nistP384.Oid.FriendlyName ||
                string.Equals(curveName, "ECDSA_P384", StringComparison.OrdinalIgnoreCase))
                return "P-384";
            else if (curveName == ECCurve.NamedCurves.nistP521.Oid.FriendlyName ||
                string.Equals(curveName, "ECDSA_P521", StringComparison.OrdinalIgnoreCase))
                return "P-521";
            throw new ArgumentException($"Unknown cryptographic curve: \"{curveName}\". Refer to https://tools.ietf.org/html/rfc7518#section-6.2.1.1 for valid values.",
                string.Join(".", nameof(curve), nameof(curve.Oid), nameof(curve.Oid.FriendlyName)));
        }
    }
}
