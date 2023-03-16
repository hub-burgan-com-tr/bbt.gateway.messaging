using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class CheckSmsStatusResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public SmsStatus status { get; set; }
    }

    public enum SmsStatus
    { 
        Delivered = 0,
        NotDelivered = 1
    }

    public enum TransactionStatus
    {
        Delivered = 0,
        NotDelivered = 1
    }

    public class CheckTransactionStatusResponse
    {
        public TransactionStatus status { get; set; }
    }
}

