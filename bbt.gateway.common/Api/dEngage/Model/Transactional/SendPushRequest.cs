using Newtonsoft.Json;
using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Transactional
{
    public class SendPushRequest
    {
        public string contactKey { get; set; }
        public string contentId { get; set; }   
        public string current { get; set; }
        public string language { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<dynamic> customParameters { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[]? Tags { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public common.Models.v2.InboxParams? inboxParams { get; set; }
    }

    public class CustomParameter
    {
        public string key { get; set; }
        public string value { get; set; }
    }

}
