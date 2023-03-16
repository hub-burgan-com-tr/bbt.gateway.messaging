using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public abstract class SendSmsRequest
    {
        public Guid Id { get; set; }

        public Phone Phone { get; set; }

        public Process Process { get; set; }
        

    }
}

