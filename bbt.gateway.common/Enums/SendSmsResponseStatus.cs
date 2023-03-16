

namespace bbt.gateway.common.Models
{
    public enum SendSmsResponseStatus 
    {
        Success = 200,        
        HasBlacklistRecord = 460,
        SimChange = 461,
        OperatorChange = 462,
        RejectedByOperator = 463,
        NotSubscriber = 464,
        ClientError = 465,
        ServerError = 466,
        MaximumCharactersCountExceed = 467,
    }

   
}