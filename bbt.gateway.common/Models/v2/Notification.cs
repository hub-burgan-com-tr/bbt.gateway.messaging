using Newtonsoft.Json;

namespace bbt.gateway.common.Models.v2
{
    public class Notification
    {
        public string notificationId { get; set; }
        public string reminderType { get; set; }
        [JsonProperty("contentHTML")]
        public string contentHtml { get; set; }
        public bool isRead { get; set; }
        public string date { get; set; }
        [JsonIgnore]
        public DateTime dateTime { get; set; }
        [JsonIgnore]
        public string customParametersString { get; set; }
        public dynamic customParameters
        {
            get
            {
                try
                {
                    return !string.IsNullOrWhiteSpace(customParametersString)
                        ? JsonConvert.DeserializeObject<dynamic>(customParametersString)
                        : customParametersString;
                }
                catch
                {
                    return customParametersString;
                }
            }
        }
    } 
}