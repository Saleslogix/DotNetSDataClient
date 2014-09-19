// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Net;

namespace Saleslogix.SData.Client.Framework
{
    public interface IAuthenticator
    {
        void Authenticate(WebRequest request);
    }
}