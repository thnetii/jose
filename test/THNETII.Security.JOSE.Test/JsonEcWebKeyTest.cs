using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using Xunit;
using System;

namespace THNETII.Security.JOSE.Test
{
    public static class JsonEcWebKeyTest
    {
        private static readonly string CookbookPublicKeyFilePath = IetfJoseCookbook.GetCookbookFilePath(@"3_1.ec_public_key.json");
        private static readonly string CookbookPrivateKeyFilePath = IetfJoseCookbook.GetCookbookFilePath(@"3_2.ec_private_key.json");
        private static readonly JsonSerializer jsonSerializer = JsonSerializer.Create();

        [Fact]
        public static void JwkCanDeserializeEcPublicKey()
        {
            JsonEcWebKey jwk = Deserialize(CookbookPublicKeyFilePath);
            Assert.NotNull(jwk);
            Assert.True(jwk.Curve.IsNamed);
            var q = jwk.Q;
            Assert.NotNull(q.X);
            Assert.NotNull(q.Y);
            Assert.Null(jwk.D);
        }

        [Fact]
        public static void JwkCanDeserializeEcPriavteKey()
        {
            JsonEcWebKey jwk = Deserialize(CookbookPrivateKeyFilePath);
            Assert.NotNull(jwk);
            Assert.True(jwk.Curve.IsNamed);
            var q = jwk.Q;
            Assert.NotNull(q.X);
            Assert.NotNull(q.Y);
            Assert.NotNull(jwk.D);
        }

        private static JsonEcWebKey Deserialize(string filepath)
        {
            using (var jsonReader = new JsonTextReader(File.OpenText(filepath)))
                return jsonSerializer.Deserialize<JsonEcWebKey>(jsonReader);
        }

        [Fact]
        public static void JwkCanImportPublicKeyToEcDsa()
        {
            JsonEcWebKey jwk = Deserialize(CookbookPublicKeyFilePath);
            ECParameters ecdsaParams;
            using (var ecdsa = ECDsa.Create())
            {
                ecdsa.ImportJsonWebKey(jwk);
                ecdsaParams = ecdsa.ExportParameters(false);
            }
            Assert.Equal(jwk.Curve.Oid?.FriendlyName, ecdsaParams.Curve.Oid?.FriendlyName);
            Assert.Equal(jwk.Q.X, ecdsaParams.Q.X);
            Assert.Equal(jwk.Q.Y, ecdsaParams.Q.Y);
            Assert.Null(ecdsaParams.D);
        }

        [Fact]
        public static void ECDsaWithNistP256CurveCanExportPublicKeyToJwk()
        {
            ECDsaCanExportToJwk(ECCurve.NamedCurves.nistP256);
        }

        [Fact]
        public static void ECDsaWithNistP384CurveCanExportPublicKeyToJwk()
        {
            ECDsaCanExportToJwk(ECCurve.NamedCurves.nistP384);
        }

        [Fact]
        public static void ECDsaWithNistP521CurveCanExportPublicKeyToJwk()
        {
            ECDsaCanExportToJwk(ECCurve.NamedCurves.nistP521);
        }

        [Fact]
        public static void ECDsaWithNistP256CurveCanExportPrivateKeyToJwk()
        {
            ECDsaCanExportToJwk(ECCurve.NamedCurves.nistP256, true);
        }

        [Fact]
        public static void ECDsaWithNistP384CurveCanExportPrivateKeyToJwk()
        {
            ECDsaCanExportToJwk(ECCurve.NamedCurves.nistP384, true);
        }

        [Fact]
        public static void ECDsaWithNistP521CurveCanExportPrivateKeyToJwk()
        {
            ECDsaCanExportToJwk(ECCurve.NamedCurves.nistP521, true);
        }

        private static void ECDsaCanExportToJwk(ECCurve curve, bool includePrivateParameters = false)
        {
            JsonEcWebKey jwk;
            string curveOidFriendlyName = curve.Oid.FriendlyName;
            using (var ecdsa = ECDsa.Create(curve))
                jwk = ecdsa.ExportJsonWebKey(includePrivateParameters);
            Assert.NotNull(jwk);
            Assert.Equal(curveOidFriendlyName, jwk.Curve.Oid?.FriendlyName);
            Assert.NotNull(jwk.Q.X);
            Assert.NotNull(jwk.Q.Y);
            if (includePrivateParameters)
                Assert.NotNull(jwk.D);
            else
                Assert.Null(jwk.D);
        }
    }
}
