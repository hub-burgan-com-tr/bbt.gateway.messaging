using System.Collections.Generic;

namespace bbt.gateway.messaging.Api.Codec.Model
{
    public class CodecSmsResponse
    {
        public CodecSmsResponseResultSet ResultSet { get; set; }
        public List<CodecSmsResponseResultListElement> ResultList { get; set; }
    }

    public class CodecSmsResponseResultSet
    { 
        public int Code { get; set; }
        public string Description { get; set; } 
    }

    public class CodecSmsResponseResultListElement
    { 
        public string SmsRefId { get; set; }
        public int Status { get; set; }
        public int ErrorCode { get; set; }
    }
}
