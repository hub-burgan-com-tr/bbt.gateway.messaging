using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models.Queue
{
    public class MailTrackingTopicModel
    {
        public string TransactionId{get;set;}
        public string Mail{get;set;}
        public DateTime CreatedAt{get;set;}
        public string CreatedByName{get;set;}
        public string CreatedByItemId{get;set;}
        public ulong CustomerNo{get;set;}
        public int ResponseCode{get;set;}
        public string DeliveryStatus{get;set;}

    }
}