namespace bbt.gateway.common.Api.dEngage.Model.Transactional
{
    public class SendBulkMailResponse
    {
        public string transactionId { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public BulkMailResponseData data { get; set; }
        
    }

    public class BulkMailResponseData 
    {
        public List<BulkMailResponseList> emailList { get; set; }
    }

    public class BulkMailResponseList
    { 
        public string groupId { get; set; }
        public List<BulkMailResponseResultList> emailResults { get; set; }

    }

    public class BulkMailResponseResultList
    { 
        public BulkMailResponseTrackingData to { get; set; }
        public BulkMailResponseTrackingData cc { get; set; }
        public BulkMailResponseTrackingData bcc { get; set; }
    }

    public class BulkMailResponseTrackingData
    {
        public string trackingId { get; set; }
        public string email { get; set; }
    }


}
