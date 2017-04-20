using System;
using System.Security.Cryptography;

namespace THNETII.Security.JOSE
{
    public static class JsonWebRsaExtensions
    {
        public static JsonRsaWebKey ExportJsonWebKey(this RSA rsa, bool includePrivateParameters)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            var param = rsa.ExportParameters(includePrivateParameters);
            var jwk = new JsonRsaWebKey
            {
                N = param.Modulus,
                E = param.Exponent
            };

            if (param.D != null)
                jwk.D = param.D;
            if (param.P != null)
                jwk.P = param.P;
            if (param.Q != null)
                jwk.Q = param.Q;
            if (param.DP != null)
                jwk.DP = param.DP;
            if (param.DQ != null)
                jwk.DQ = param.DQ;
            if (param.InverseQ != null)
                jwk.QI = param.InverseQ;

            return jwk;
        }
    }
}
