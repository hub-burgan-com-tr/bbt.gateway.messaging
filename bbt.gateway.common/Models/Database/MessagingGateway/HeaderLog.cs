using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class HeaderLog
    {
        public Guid Id { get; set; }
        public Header Header { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public string ParameterMaster { get; set; }
        public string ParameterSlave { get; set; }
        public Process CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
