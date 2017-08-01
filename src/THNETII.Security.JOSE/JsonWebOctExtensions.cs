using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using static THNETII.Security.JOSE.JsonWebAlgorithm;

namespace THNETII.Security.JOSE
{
    public static class JsonWebOctExtensions
    {
        public static JsonOctWebKey ExportJsonWebKey(this KeyedHashAlgorithm hash)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash));

            return new JsonOctWebKey { Key = hash.Key };
        }

        public static void ImportJsonWebKey(this KeyedHashAlgorithm hash, JsonOctWebKey jwk)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash));
            else if (jwk == null)
                throw new ArgumentNullException(nameof(jwk));

            hash.Key = jwk.Key;
        }
    }
}
