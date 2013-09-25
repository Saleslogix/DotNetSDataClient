// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;

#if !PCL && !SILVERLIGHT
using System.Runtime.Serialization;
#endif

namespace Saleslogix.SData.Client
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