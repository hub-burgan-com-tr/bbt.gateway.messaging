using bbt.gateway.common.Models;

namespace bbt.gateway.messaging.Api.Pusula.Model.GetCustomer
{
    public class GetCustomerResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int BranchCode { get; set; }
        public string BusinessLine { get; set; }
        public Phone MainPhone { get; set; } = new();
        public string MainEmail { get; set; }
        public string CitizenshipNo { get; set; }
    }
}
