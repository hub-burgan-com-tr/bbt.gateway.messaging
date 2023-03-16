namespace bbt.gateway.messaging.ui.Data
{
    public class QueryParams
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int smsType { get; set; }
        public string orderBy { get; set; }
        public string createdName { get; set; }=string.Empty;

    }
}
