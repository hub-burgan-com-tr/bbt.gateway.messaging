namespace bbt.gateway.messaging.ui.Data
{
    public class SearchModel
    {
        public int SelectedSearchType { get; set; } = 1;
        public string FilterValue { get; set; } = String.Empty;
        public string CreatedBy { get; set; } = String.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public MessageTypeEnum MessageType { get; set; }
        public SmsTypeEnum SmsType { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string OrderBy { get; set; }
    }
}
