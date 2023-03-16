namespace bbt.gateway.messaging.ui.Data
{
    public class MessageType
    { 
        public string Name { get; set; }
        public MessageTypeEnum Type { get; set; }
    }
    public enum MessageTypeEnum
    {
        Unselected,
        Sms,
        Mail,
        PushNotification
    }
}
