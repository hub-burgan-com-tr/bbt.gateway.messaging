using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Pusula.Model.GetByPhone
{
    public class GetByPhoneNumberRequest
    {
        public int CountryCode { get; set; }
        public int CityCode { get; set; }
        public int TelephoneNumber { get; set; }
    }
}
