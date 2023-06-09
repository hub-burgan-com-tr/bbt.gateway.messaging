﻿using System.Collections.Generic;

namespace bbt.gateway.messaging.Api.dEngage.Model.Transactional
{
    public class SendSmsRequest
    {
        public Content content { get; set; } = new();
        public Send send { get; set; } = new();
        public string current { get; set; }
        public List<string> tags { get; set; } = new();
    }

    public class Content
    {
        public string smsFromId { get; set; }
        public string message { get;set; }
        public string templateId { get; set; }
    }

    public class Send 
    {
        public string to { get; set; }
    }

}
