using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace THNETII.Security.JOSE.Test
{
    public class JsonWebKeyTypeConverterTest
    {
        [Theory]
        [InlineData("RSA", typeof(JsonRsaWebKey))]
        [InlineData("EC", typeof(JsonEcWebKey))]
        [InlineData("oct", typeof(JsonOctWebKey))]
        public void JsonWebDeserializerToSpecificType(string ktyString, Type expectType)
        {
            var json = JObject.FromObject(new { kty = ktyString });
            JsonWebKey jwk;
            using (var reader = json.CreateReader())
                jwk = JsonSerializer.Create().Deserialize<JsonWebKey>(reader);
            Assert.IsType(expectType, jwk);
        }
    }
}
