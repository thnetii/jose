using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Reflection;
using THNETII.Common.DataContractSerializer;

namespace THNETII.Security.JOSE
{
    public class JsonWebKeyConverter : JsonConverter
    {
        private static readonly Type jsonWebKeyType = typeof(JsonWebKey);
        private static readonly TypeInfo jsonWebKeyTypeInfo = jsonWebKeyType.GetTypeInfo();
        private static readonly EnumConverter ktyConvert = new EnumConverter(jsonWebKeyType);

        public override bool CanConvert(Type objectType)
            => jsonWebKeyTypeInfo.IsAssignableFrom(objectType?.GetTypeInfo());

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObj = JObject.Load(reader);
            var ktyValue = EnumStringConverter.TryParse<JsonWebKeyType>(jsonObj.Value<string>(JsonWebKey.KeyTypeJsonPropertyName), out var parsed) ? parsed : default(JsonWebKeyType);
            JsonWebKey jwk;
            switch (ktyValue)
            {
                case JsonWebKeyType.Rsa:
                    jwk = new JsonRsaWebKey();
                    break;
                case JsonWebKeyType.Ec:
                    jwk = new JsonEcWebKey();
                    break;
                case JsonWebKeyType.Oct:
                    jwk = new JsonOctWebKey();
                    break;
                default:
                    jwk = new JsonWebKey();
                    break;
            }
            using (var objReader = jsonObj.CreateReader())
                serializer.Populate(objReader, jwk);
            return jwk;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) 
            => serializer.Serialize(writer, value);
    }
}
