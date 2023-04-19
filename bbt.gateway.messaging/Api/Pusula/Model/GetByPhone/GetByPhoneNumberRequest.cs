using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.messaging.Api.Pusula.Model.GetByPhone
{
    public class GetByPhoneNumberRequest
    {
        public string CountryCode { get; set; }
        public string CityCode { get; set; }
        public string TelephoneNumber { get; set; }
    }
}
