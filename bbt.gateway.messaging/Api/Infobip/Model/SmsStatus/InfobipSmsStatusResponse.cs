using System.Collections.Generic;

namespace bbt.gateway.messaging.Api.Infobip.Model.SmsStatus
{
    public class InfobipSmsStatusResponse
    {
        public List<Result> results { get; set; }
    }

    public class Result
    {
        public Status status { get; set; }
        public Error error { get; set; }
    }

    public class Status
    {
        public int groupId { get; set; }
        public string groupName { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Error
    {
        public int groupId { get; set; }
        public string groupName { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool permanent { get; set; }
    }
}
