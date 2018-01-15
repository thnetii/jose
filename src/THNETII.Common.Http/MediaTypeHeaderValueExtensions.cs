using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace THNETII.Common.Http
{
    public static class MediaTypeHeaderValueExtensions
    {
        public static bool IsHtml(this MediaTypeHeaderValue contentType, bool trueIfNoMediaType = true)
        {
            if (contentType == null)
                return trueIfNoMediaType;
            else if (string.IsNullOrWhiteSpace(contentType.MediaType))
                return trueIfNoMediaType;
            return contentType.MediaType.ToUpperInvariant().Contains(HttpWellKnownMediaType.ContainsUppercase.Html);
        }
    }
}
