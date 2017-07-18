using System;
using System.Collections.Generic;
using System.Text;

namespace THNETII.Security.JOSE
{
    public class JsonEcWebKey : JsonWebKey
    {
        public JsonEcWebKey() : base(JsonWebKeyType.Ec) { }
    }
}
