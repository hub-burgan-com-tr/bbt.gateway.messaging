using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class AddWhitelistRequest
    {
        public Phone Phone { get; set; }
        public string Email { get; set; }
        public Process CreatedBy { get; set; }
    }
}
