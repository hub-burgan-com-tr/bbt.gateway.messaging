namespace bbt.gateway.messaging.ui.Data
{
    public class SmsType
    {
        public string Name { get; set; }
        public SmsTypeEnum Type { get; set; }
    }


    public enum SmsTypeEnum
    {
        Unselected,
        Otp,
        Transactional
    }
}
