using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Settings
{
    public class GetSmsFromsResponse
    {
        public SmsFroms data { get; set; }
    }

    public class SmsFroms
    { 
        public List<SmsFrom> smsFroms { get; set; }
    }
    public class SmsFrom
    { 
        public string id { get; set; }
        public string type { get; set; }
        public string sender { get; set; }
        public bool isDefault { get; set; }
        public string partnerName { get; set; }
    }
}
