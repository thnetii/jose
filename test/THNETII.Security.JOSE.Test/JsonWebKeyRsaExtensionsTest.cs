using System.Security.Cryptography;
using Xunit;

namespace THNETII.Security.JOSE.Test
{
    public static class JsonWebKeyRsaExtensionsTest
    {
        [Fact]
        public static void CreatesEqualJsonWebKeyFromPublicExportedRsa()
        {
            var rsa = RSA.Create();
            var rsaParams = rsa.ExportParameters(includePrivateParameters: false);

            var jwk = rsa.ExportJsonWebKey(includePrivateParameters: false);

            Assert.Equal(JsonWebKeyType.Rsa, jwk.KeyType);
            Assert.Equal("RSA", jwk.KeyTypeString, ignoreCase: true);
            Assert.Equal(rsaParams.Modulus, jwk.N);
            Assert.Equal(rsaParams.Exponent, jwk.E);
        }
    }
}
