using System;

#if !PCL && !SILVERLIGHT
using System.Runtime.Serialization;
#endif

namespace Sage.SData.Client
{
    [Serializable]
    public class SDataClientException : Exception
    {
        public SDataClientException()
        {
        }

        public SDataClientException(string message)
            : base(message)
        {
        }

        public SDataClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        protected SDataClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}