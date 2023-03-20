using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Transactional
{
    public class MailStatusResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public MailStatusData data { get; set; }
    }

    public class MailResult
    {
        public string event_type { get; set; }
        public string event_date { get; set; }
        public List<MailEvent> events { get; set; }
        
    }

    public class MailStatusData
    { 
      public List<MailResult> result { get; set; }  
    }

    public class MailEvent
    {
        public string event_type { get; set; }
        public string event_date { get; set; }
        public string bounce_code { get; set; }
        public string bounce_text { get; set; }
    }
}
