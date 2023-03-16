using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Pusula.Model.GetByPhone
{
    public class GetByPhoneNumberResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public ulong CustomerNo { get; set; }
    }
}
