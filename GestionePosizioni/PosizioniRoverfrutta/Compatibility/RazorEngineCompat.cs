using System;

namespace RazorEngine.Templating
{
    public class TemplateServiceException : Exception
    {
        public TemplateServiceException(string message) : base(message) { }
        public TemplateServiceException(string message, Exception inner) : base(message, inner) { }
    }

    public class DynamicViewBag
    {
    }
}

namespace Raven.Client
{
    public enum EscapeQueryOptions
    {
        EscapeAll,
        AllowAllWildcards,
        RawQuery
    }
}

namespace Raven.Client.Linq
{
}
