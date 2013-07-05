using System.Net;
using Sage.SData.Client.Framework;

namespace Sage.SData.Client
{
    public interface ISDataClient
    {
        string Uri { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string UserAgent { get; set; }
        int? Timeout { get; set; }
        int? TimeoutRetryAttempts { get; set; }
        IWebProxy Proxy { get; set; }
        ICredentials Credentials { get; set; }
        INamingScheme NamingScheme { get; set; }
        MediaType? Format { get; set; }
        string Language { get; set; }

        ISDataResults Execute(ISDataParameters parms);
        ISDataResults<T> Execute<T>(ISDataParameters parms);
    }
}