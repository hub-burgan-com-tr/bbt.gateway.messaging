using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Contents
{
    public class PushContentsResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public PushResult data { get; set; }
        
    }

    public class PushResult
    {
        public List<PushContentInfo> result { get; set; }
        public bool queryForNextPage { get; set; }
        public int totalRowCount { get; set; }
    }

    public class PushContentInfo : IContentReadeble
    {
        public string name { get; set; }
        public string id { get; set; }
        public string location { get; set; }
        public string GetPath(bool isAbsolutePath)
        {
            if (isAbsolutePath)
                return $"{location}/{name}";
            return name;
        }
    }
}
