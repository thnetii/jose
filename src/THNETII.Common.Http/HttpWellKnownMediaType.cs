namespace THNETII.Common.Http
{
    public static class HttpWellKnownMediaType
    {
        public static class Key
        {
            public const string Text = "text";
            public const string Application = "application";

            public const string Plain = "plain";
            public const string Html = "html";
        }

        public static class ContainsUppercase
        {
            public static readonly string Html = Key.Html.ToUpperInvariant();
        }

        public const string Html = Key.Plain + "/" + Key.Html;
    }
}
