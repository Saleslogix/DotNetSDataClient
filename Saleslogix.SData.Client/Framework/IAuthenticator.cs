// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Net;

namespace Saleslogix.SData.Client.Framework
{
    public interface IAuthenticator
    {
        void Authenticate(WebRequest request);
        ///invoked in response to 401
        UnauthorizedAction Unauthorized(WebResponse response);
    }

    public interface IAuthenticator2 : IAuthenticator
    {
        ///invoked in response to 403
        UnauthorizedAction Forbidden(WebResponse response);
    }

    public enum UnauthorizedAction
    {
        Throw,
        Retry
    }
}
