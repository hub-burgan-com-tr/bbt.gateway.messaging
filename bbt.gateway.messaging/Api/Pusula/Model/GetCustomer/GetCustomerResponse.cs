using bbt.gateway.common.Models;
using System.Collections.Generic;

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
        public List<string> VerifiedMailAdresses { get; set; } = new();
        public string CustomerProfile { get; set; }
    }
}