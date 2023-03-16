using bbt.gateway.common.Models;

namespace bbt.gateway.messaging.Api
{
    public class OperatorApiTrackingResponse
    {
        public OperatorType OperatorType { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseBody { get; set; }
    }
}
