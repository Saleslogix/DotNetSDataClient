// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.IO;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client.Content
{
    public class TextContentHandler : IContentHandler
    {
        public object ReadFrom(Stream stream)
        {
            Guard.ArgumentNotNull(stream, "stream");

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void WriteTo(object obj, Stream stream, INamingScheme namingScheme = null)
        {
            Guard.ArgumentNotNull(obj, "obj");
            Guard.ArgumentNotNull(stream, "stream");

            var writer = new StreamWriter(stream);
            writer.Write(obj.ToString());
            writer.Flush();
        }
    }
}