using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Contents
{
    public class MailContentsResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public Result data { get; set; }
    }

    public class Result
    {
        public List<ContentInfo> result { get; set; }
        public bool queryForNextPage { get; set; }
        public int totalRowCount { get; set; }
    }

    public class ContentInfo : IContentReadeble
    {
        public string contentName { get; set; }
        public string publicId { get; set; }
        public string location { get; set; }
        public bool isTransactionalContent { get; set; }
        public string GetPath(bool isAbsolutePath)
        {
            if(isAbsolutePath)
                return $"{location}/{contentName}";
            return contentName;
        }

    }
}
