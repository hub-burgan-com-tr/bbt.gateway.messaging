using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Contents
{
    public class PushContentResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public PushContentData data { get; set; }
    }

    public class PushContentData
    {
        public PushContentDetail contentDetail { get; set; }
    }

    public class PushContentDetail
    { 
        public List<PushContent> contents { get; set; }
    }

    public class PushContent
    {
        public string language { get; set; }
        public string title { get; set; }
        public string message { get; set; }
    }
}
