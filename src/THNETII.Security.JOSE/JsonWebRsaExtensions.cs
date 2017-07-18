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
        /// Imports the specified JSON Web Key as parameters for the given RSA instance.
        /// </summary>
        /// <param name="rsa">The RSA cryptographic instance into which the key will be imported. Must not be <c>null</c>.</param>
        /// <param name="jwk">The JSON RSA Web Key to import. Must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">Either <paramref name="rsa"/> or <paramref name="jwk"/> is <c>null</c>.</exception>
        /// <exception cref="CryptographicException">The <paramref name="jwk"/> parameter has missing fields.</exception>
        public static void ImportJsonWebKey(this RSA rsa, JsonRsaWebKey jwk)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));
            else if (jwk == null)
                throw new ArgumentNullException(nameof(jwk));
            try
            {
                rsa.ImportParameters(new RSAParameters()
                {
                    Modulus = jwk.N,
                    Exponent = jwk.E,
                    D = jwk.D,
                    P = jwk.P,
                    Q = jwk.Q,
                    DP = jwk.DP,
                    DQ = jwk.DQ,
                    InverseQ = jwk.QI
                });
            }
            catch (CryptographicException cryptoExcept)
            { throw new CryptographicException($"The {nameof(jwk)} parameter has missing fields.", cryptoExcept); }
        }
    }
}
