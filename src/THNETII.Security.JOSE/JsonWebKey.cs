using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using THNETII.Common;

namespace THNETII.Security.JOSE
{
    [DataContract]
    public class JsonWebKey
    {
        private readonly DuplexConversionTuple<string, JsonWebKeyType> kty =
            new DuplexConversionTuple<string, JsonWebKeyType>(ConvertStringToKty, ConvertKtyToString);

        [DataMember(Name = "kty")]
        public virtual string KeyTypeString
        {
            get => kty.RawValue;
            set => kty.RawValue = value;
        }

        [IgnoreDataMember]
        public virtual JsonWebKeyType KeyType
        {
            get => kty.ConvertedValue;
            set => kty.ConvertedValue = value;
        }

        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

        private static JsonWebKeyType ConvertStringToKty(string rawKty)
        {
            if (Enum.TryParse(rawKty, ignoreCase: true, result: out JsonWebKeyType kty))
                return kty;
            else
                return JsonWebKeyType.Unknown;
        }

        private static string ConvertKtyToString(JsonWebKeyType kty)
        {
            switch (kty)
            {
                case JsonWebKeyType.Ec: return "EC";
                case JsonWebKeyType.Rsa: return "RSA";
                default: return null;
            }
        }
    }
}
