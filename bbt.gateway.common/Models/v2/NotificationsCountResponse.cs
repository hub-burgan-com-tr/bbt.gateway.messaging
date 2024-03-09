using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models.v2
{
    public class NotificationsCountResponse
    {
        public int readCount { get; set; }
        public int unreadCount { get; set; }
    }
}
