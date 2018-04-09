using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace THNETII.Security.JOSE
{
    public static class JoseEcDsaExtensions
    {
        public static JsonEcWebKey ExportJsonWebKey(this ECDsa ecdsa, bool includePrivateParameters)
        {
            if (ecdsa == null)
                throw new ArgumentNullException(nameof(ecdsa));

            var ecdsparams = ecdsa.ExportParameters(includePrivateParameters);
            var jwk = new JsonEcWebKey
            {
                Curve = ecdsparams.Curve,
                Q = ecdsparams.Q
            };
            if (ecdsparams.D != null)
                jwk.D = ecdsparams.D;

            return jwk;
        }

        public static void ImportJsonWebKey(this ECDsa ecdsa, JsonEcWebKey jwk)
        {
            if (ecdsa == null)
                throw new ArgumentNullException(nameof(ecdsa));
            else if (jwk == null)
                throw new ArgumentNullException(nameof(jwk));
            ecdsa.ImportParameters(new ECParameters
            {
                Curve = jwk.Curve,
                Q = jwk.Q,
                D = jwk.D
            });
        }
    }
}
