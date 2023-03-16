using bbt.gateway.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbt.gateway.messaging.Api.Vodafone.Model
{
    public class VodafoneSmsRequest
    {
        public string User { get; set; }
        public string AuthToken { get; set; }
        public string PhoneNo { get; set; }
        public string Message { get; set; }
        public string Header { get; set; }
        public string ExpiryPeriod { get; set; }

        public string ControlHour { get; set; }
        public bool IsAbroad { get; set; }
    }
}
