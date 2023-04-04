using System.Collections.Generic;

namespace bbt.gateway.common.Api.dEngage.Model.Transactional
{
    public class SendBulkMailRequest
    {
        public List<BulkMailItem> emailList { get; set; }
    }

    public class BulkMailItem
    { 
        public ContentMail content { get; set; }
        public List<BulkMailProp> props { get; set; }
    }

    public class BulkMailProp
    {
        public SendMail send { get; set; } = new();
        public List<Attachment> attachments { get; set; }
        public string current { get; set; }
    }


}
