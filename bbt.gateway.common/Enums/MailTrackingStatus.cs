

namespace bbt.gateway.common.Models
{
    public enum MailTrackingStatus
    {
        Delivered = 200,        
        HardBounced = 201,
        SoftBounced= 202,
        Opened = 203,
        Clicked = 204,
        Spam = 205,
        Pending = 206,
    }
}