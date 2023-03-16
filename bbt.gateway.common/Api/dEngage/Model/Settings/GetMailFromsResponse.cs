using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Settings
{
    public class GetMailFromsResponse
    {
        public MailFroms data { get; set; }
    }

    public class MailFroms
    { 
        public List<MailFrom> emailFroms { get; set; }
    }
    public class MailFrom
    { 
        public string id { get; set; }
        public string fromName { get; set; }
        public string fromAddress { get; set; }
        public bool isDefault { get; set; }
    }
}
