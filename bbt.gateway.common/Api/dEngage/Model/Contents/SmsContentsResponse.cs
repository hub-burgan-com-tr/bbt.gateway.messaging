using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Contents
{
    public class SmsContentsResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public SmsResult data { get; set; }
        
    }

    public class SmsResult
    {
        public List<SmsContentInfo> result { get; set; }
        public bool queryForNextPage { get; set; }
        public int totalRowCount { get; set; }
    }

    public class SmsContentInfo : IContentReadeble
    {
        public string contentName { get; set; }
        public string publicId { get; set; }

        public string location { get; set; }
        public bool isTransactionalContent { get; set; }
        public string GetPath(bool isAbsolutePath)
        {
            if (isAbsolutePath)
                return $"{location}/{contentName}";
            return contentName;
        }
    }
}
