namespace bbt.gateway.messaging.ui.Data
{
    public class FilterInput
    {
        public string Name { get; set; }
        public string Helpline { get; set; }

        public List<MessageType> MessageTypes { get; set; }
        public List<SmsType> SmsTypes { get; set; }
    }
}
