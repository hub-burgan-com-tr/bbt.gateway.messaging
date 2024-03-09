using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Api.Reminder.Model
{
    public class NotificationInfo
    {
        public string notificationId { get; set; }
        public string reminderType { get; set; }
        public string contentHTML { get; set; }
        public bool isRead { get; set; }
        public string date { get; set; }
    }
}
