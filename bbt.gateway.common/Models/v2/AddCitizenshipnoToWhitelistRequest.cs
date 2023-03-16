using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models.v2
{
    public class AddCitizenshipnoToWhitelistRequest
    {
        public string CitizenshipNo { get; set; }
        public Process CreatedBy { get; set; }
    }
}
