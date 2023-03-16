using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Contents
{
    public class SmsContentResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public SmsContentData data { get; set; }
        
    }

    public class SmsContentData
    {
        public SmsContentDetail contentDetail { get; set; }
    }

    public class SmsContentDetail
    { 
        public List<SmsContent> contents { get; set; }
    }

    public class SmsContent
    {
        public string language { get; set; }
        public string message { get; set; }
        public string senderName { get; set; }
    }
}
