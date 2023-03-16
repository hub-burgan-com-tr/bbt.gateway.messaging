using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models.v2
{
    public class AddMailToWhitelistRequest
    {
        public string Email { get; set; }
        public Process CreatedBy { get; set; }
    }
}
