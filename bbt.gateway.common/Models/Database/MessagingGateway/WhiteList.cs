using bbt.gateway.common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class WhiteList
    {
        public Guid Id { get; set; }
        public string Mail { get; set; }
        public string ContactId { get; set; }
        public Phone Phone { get; set; }
        public Process CreatedBy { get; set; }
        public string IpAddress { get; set; }
    }
   
}
