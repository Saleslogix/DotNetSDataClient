#if PCL
namespace System.ComponentModel
{
    internal class BrowsableAttribute : Attribute
    {
// ReSharper disable UnusedParameter.Local
        public BrowsableAttribute(bool browsable)
// ReSharper restore UnusedParameter.Local
        {
        }
    }
}
#endif

#if PCL || SILVERLIGHT
namespace System
{
    internal interface ICloneable
    {
// ReSharper disable UnusedMember.Global
        object Clone();
// ReSharper restore UnusedMember.Global
    }

    internal class SerializableAttribute : Attribute
    {
    }

    namespace Net
    {
        using Sage.SData.Client.Utilities;

        internal static class WebHeaderCollectionExtensions
        {
            public static void Add(this WebHeaderCollection headers, string header)
            {
                Guard.ArgumentNotNull(header, "header");
                var index = header.IndexOf(':');
                if (index < 0)
                {
                    throw new ArgumentException("Colon missing in web header", "header");
                }
                var name = header.Substring(0, index);
                var value = header.Substring(index + 1);
                headers[name] = value;
            }
        }
    }

    namespace Runtime.Serialization
    {
        internal interface ISerializable
        {
        }
    }

    namespace Text
    {
        internal static class EncodingExtensions
        {
            public static string GetString(this Encoding encoding, byte[] bytes)
            {
                return encoding.GetString(bytes, 0, bytes.Length);
            }
        }
    }
}
#endif

#if NET_2_0 || NET_3_5
namespace System.IO
{
    internal static class StreamExtensions
    {
        public static void CopyTo(this Stream source, Stream destination)
        {
            var buffer = new byte[0x1000];
            int num;
            while ((num = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, num);
            }
        }
    }
}
#endif