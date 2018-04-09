using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace THNETII.Security.JOSE
{
    /// <summary>
    /// Provides extension methods for interaction between JOSE and the .NET Standard built-in <see cref="RSA"/> crypto provider.
    /// </summary>
    public static class JoseRsaExtensions
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
        public static void ImportJsonWebKey(this RSA rsa, JsonRsaWebKey jwk)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));
            else if (jwk == null)
                throw new ArgumentNullException(nameof(jwk));
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

        private static void PrepareJsonWebSignatureHeader(Action<JsonWebSignatureHeader> signatureHeaderConfiguration, JsonWebSignatureHeader protected_header, out HashAlgorithmName hashAlgName, out RSASignaturePadding rsaSigPadding)
        {
            signatureHeaderConfiguration?.Invoke(protected_header);
            switch (protected_header.Algorithm)
            {
                case JsonWebAlgorithm.RS256:
                    hashAlgName = HashAlgorithmName.SHA256;
                    rsaSigPadding = RSASignaturePadding.Pkcs1;
                    break;
                case JsonWebAlgorithm.RS384:
                    hashAlgName = HashAlgorithmName.SHA384;
                    rsaSigPadding = RSASignaturePadding.Pkcs1;
                    break;
                case JsonWebAlgorithm.RS512:
                    hashAlgName = HashAlgorithmName.SHA512;
                    rsaSigPadding = RSASignaturePadding.Pkcs1;
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported JSON Web Signature Algorithm for RSA-type signatures: {protected_header.AlgorithmName}");
            }
        }

        public static JsonWebSignature SignDataAsJsonWebSignature(this RSA rsa, byte[] payload, Action<JsonWebSignatureHeader> signatureHeaderConfiguration = null)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            var protected_header = new JsonWebSignatureHeader
            {
                Algorithm = JsonWebAlgorithm.RS256,
                JsonWebKey = rsa.ExportJsonWebKey(includePrivateParameters: false),
                SignatureMediaTypeString = JoseMediaTypes.JoseJsonShortMediaType
            };
            PrepareJsonWebSignatureHeader(signatureHeaderConfiguration, protected_header,
                out HashAlgorithmName hashAlgName, 
                out RSASignaturePadding rsaSigPadding
                );

            var jws = new JsonWebSignature
            {
                ProtectedHeader = protected_header,
                Payload = payload
            };

            var signatureInput = Encoding.ASCII.GetBytes(jws.GetSignatureInput());
            var signatureBytes = rsa.SignData(signatureInput, hashAlgName, rsaSigPadding);
            jws.Signature = signatureBytes;

            return jws;
        }

        public static void AddSignatureToJsonWebSignature(this RSA rsa, JsonWebSignature jws, Action<JsonWebSignatureHeader> signatureHeaderConfiguration = null)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));
            else if (jws == null)
                throw new ArgumentNullException(nameof(jws));

            var protected_header = new JsonWebSignatureHeader
            {
                Algorithm = JsonWebAlgorithm.RS256,
                JsonWebKey = rsa.ExportJsonWebKey(includePrivateParameters: false)
            };
            PrepareJsonWebSignatureHeader(signatureHeaderConfiguration, protected_header,
                out HashAlgorithmName hashAlgName,
                out RSASignaturePadding rsaSigPadding
                );

            var signatureInput = Encoding.ASCII.GetBytes(jws.GetSignatureInput());
            var signatureBytes = rsa.SignData(signatureInput, hashAlgName, rsaSigPadding);

            jws.ProtectedHeader = null;
            jws.UnprotectedHeader = null;
            jws.Signature = null;
            if (jws.Signatures == null)
                jws.Signatures = new List<JsonWebSignatureFlat>(1);
            jws.Signatures.Add(new JsonWebSignatureFlat
            {
                ProtectedHeader = protected_header,
                Signature = signatureBytes
            });
        }
    }
}
