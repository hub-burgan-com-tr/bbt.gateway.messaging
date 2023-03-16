using System;
using System.Collections.Generic;

namespace bbt.gateway.messaging.Api.Codec.Model
{
    public class CodecSmsStatusResponse
    {
        public CodecSmsStatusResponseResultSet ResultSet { get; set; }
        public List<CodecSmsStatusResponseResultListElement> ResultList { get; set; }
    }

    public class CodecSmsStatusResponseResultSet
    { 
        public int Code { get; set; }
        public string Description { get; set; } 
    }

    public class CodecSmsStatusResponseResultListElement
    { 
        public string SmsRefId { get; set; }
        public int Status { get; set; }
        public int ErrorCode { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
