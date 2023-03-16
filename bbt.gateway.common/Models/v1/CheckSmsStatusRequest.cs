using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class CheckSmsStatusRequest
    {
        public Guid TxnId { get; set; }
        

    }
}

