using bbt.gateway.common.Models;

namespace bbt.gateway.common.GlobalConstants
{
    public class GlobalConstants
    {
        public static readonly string DAPR_STATE_STORE = "messaginggateway-statestore";
        public static readonly string DAPR_SECRET_STORE = "messaginggateway-secretstore";

        public static readonly string SMS_CONTENTS_SUFFIX = "SmsContents";
        public static readonly string MAIL_CONTENTS_SUFFIX = "MailContents";
        public static readonly string PUSH_CONTENTS_SUFFIX = "PushContents";

        public static readonly Dictionary<int,OperatorReportInfo> reportOperators = new Dictionary<int, OperatorReportInfo>()
        {
            {1, new(){ OperatorType = OperatorType.Turkcell,isFast = false,isOtp = true,AdditionalOperatorType = null} },
            {2, new(){ OperatorType = OperatorType.Vodafone,isFast = false,isOtp = true,AdditionalOperatorType = null} },
            {3,new(){ OperatorType = OperatorType.TurkTelekom,isFast = false,isOtp = true,AdditionalOperatorType = null} },
            {4,new(){ OperatorType = OperatorType.Codec,isFast = true,isOtp = false,AdditionalOperatorType = null} },
            {5,new(){ OperatorType = OperatorType.dEngageBurgan,isFast = true,isOtp = false,
                AdditionalOperatorType = new(){ OperatorType = OperatorType.dEngageBurgan,isFast = true,isOtp = false,AdditionalOperatorType = null}} },
        };
    }
}
