using Newtonsoft.Json;
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

        public static JsonRsaWebKey ToRsaWebKey(this JsonWebKey jwk_general)
        {
            if (jwk_general == null)
                return null;
            else if (jwk_general is JsonRsaWebKey jwk_rsa)
                return jwk_rsa;
            else
            {
                var jwk_json = JsonConvert.SerializeObject(jwk_general);
                return JsonConvert.DeserializeObject<JsonRsaWebKey>(jwk_json);
            }
        }
    }
}
