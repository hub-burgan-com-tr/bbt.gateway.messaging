using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Contents
{
    public class MailContentResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public MailContentData data { get; set; }

    }

    public class MailContentData
    {
        public MailContentDetail contentDetail { get; set; }
    }

    public class MailContentDetail
    { 
        public List<MailContent> contents { get; set; }
    }

    public class MailContent
    {
        public string language { get; set; }
        public string content { get; set; }
        public string subject { get; set; }
        public string fromName { get; set; }
        public string fromAddress { get; set; }
    }
}
