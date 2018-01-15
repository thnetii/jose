using System.Net.Http;

namespace THNETII.Common.Http
{
    public static class HttpContentExtensions
    {
        public static bool IsHtml(this HttpContent httpContent, bool trueIfNoMediaType = true)
        {
            if (httpContent == null)
                return false;
            return httpContent.Headers.ContentType.IsHtml(trueIfNoMediaType);
        }
    }
}
