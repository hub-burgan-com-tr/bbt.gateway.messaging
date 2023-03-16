using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Transactional
{
    public class SmsStatusResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public StatusData data { get; set; }
    }

    public class Result
    {
        public string gsm { get; set; }
        public string event_type { get; set; }
        public string event_date { get; set; }
        public string senderName { get; set; }
        public string partnerName { get; set; }
        public string created_at { get; set; }
    }

    public class StatusData
    { 
      public List<Result> result { get; set; }  
    }
}
