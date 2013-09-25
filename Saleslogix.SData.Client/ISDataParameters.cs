using System.Collections.Generic;
using Saleslogix.SData.Client.Framework;

namespace Saleslogix.SData.Client
{
    public interface ISDataParameters
    {
        MediaType[] Accept { get; set; }
        HttpMethod Method { get; set; }
        string Selector { get; set; }
        object Content { get; set; }
        MediaType? ContentType { get; set; }
        string ETag { get; set; }
        IDictionary<string, string> Form { get; }
        IList<AttachedFile> Files { get; }
        string Path { get; }
        int? StartIndex { get; set; }
        int? Count { get; set; }
        string Where { get; set; }
        string OrderBy { get; set; }
        string Search { get; set; }
        string Include { get; set; }
        int? Precedence { get; set; }
        string Select { get; set; }
        bool? IncludeSchema { get; set; }
        bool? ReturnDelta { get; set; }
        string TrackingId { get; set; }
        MediaType? Format { get; set; }
        string Language { get; set; }
        string Version { get; set; }
    }
}