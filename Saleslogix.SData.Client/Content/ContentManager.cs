using System.Collections.Generic;
using Saleslogix.SData.Client.Framework;

namespace Saleslogix.SData.Client.Content
{
    public static class ContentManager
    {
        private static readonly IDictionary<MediaType, IContentHandler> _contentHandlers = new Dictionary<MediaType, IContentHandler>();

        static ContentManager()
        {
            var atomHandler = new AtomContentHandler();
            SetHandler(MediaType.Atom, atomHandler);
            SetHandler(MediaType.AtomEntry, atomHandler);
            SetHandler(MediaType.Json, new JsonContentHandler());
            SetHandler(MediaType.Xml, new XmlContentHandler());
            SetHandler(MediaType.Text, new TextContentHandler());
        }

        public static IContentHandler GetHandler(MediaType contentType)
        {
            IContentHandler handler;
            _contentHandlers.TryGetValue(contentType, out handler);
            return handler;
        }

        public static void SetHandler(MediaType contentType, IContentHandler handler)
        {
            _contentHandlers[contentType] = handler;
        }
    }
}