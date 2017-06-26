using Newtonsoft.Json;
using System;
using System.Security.Cryptography;

namespace THNETII.Security.JOSE
{
    /// <summary>
    /// Provides extension methods for interaction with RSA-type JSON Web Key objects.
    /// </summary>
    public static class JsonWebRsaExtensions
    {
        /// <summary>
        /// Exports the RSA instance to a RSA-type JSON Web Key object, if specified including the private parameters of the RSA encryption object.
        /// </summary>
        /// <param name="rsa">The RSA cryptographic instance to export. Must not be <c>null</c>.</param>
        /// <param name="includePrivateParameters"><c>true</c> to include RSA private parameters in the returned JWK object, <c>false</c> to only include public parameters.</param>
        /// <returns>A <see cref="JsonRsaWebKey"/> instance containing the RSA parameters for use with JOSE operations.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rsa"/> is <c>null</c>.</exception>
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

        /// <summary>
        /// Converts the JSON Web Key instance into an RSA-type JSON Web Key.
        /// </summary>
        /// <param name="jwk_general">The JSON Web Key instance to convert.</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><c>null</c> if <paramref name="jwk_general"/> is <c>null</c>.</item>
        /// <item>The exact same instance as specified in the <paramref name="jwk_general"/> paramenter, if <paramref name="jwk_general"/> already is an instance of the <see cref="JsonRsaWebKey"/> class.</item>
        /// <item>A new instance of the <see cref="JsonRsaWebKey"/> class, that is deserialized from the JSON string produced by serializing <paramref name="jwk_general"/>.</item>
        /// </list>
        /// </returns>
        /// <exception cref="InvalidCastException">Unable to convert <paramref name="jwk_general"/> to an RSA JSON Web Key object. The Exception message contains the JSON representation of <paramref name="jwk_general"/>.</exception>
        public static JsonRsaWebKey ToRsaWebKey(this JsonWebKey jwk_general)
        {
            if (jwk_general == null)
                return null;
            else if (jwk_general is JsonRsaWebKey jwk_rsa)
                return jwk_rsa;
            else
            {
                var jwk_json = JsonConvert.SerializeObject(jwk_general);
                try { return JsonConvert.DeserializeObject<JsonRsaWebKey>(jwk_json); }
                catch (ArgumentException argExcept) { throw new InvalidCastException($"Unable to convert to an RSA JSON Web Key object.{Environment.NewLine}{jwk_json}", argExcept); }
            }
        }
    }
}
