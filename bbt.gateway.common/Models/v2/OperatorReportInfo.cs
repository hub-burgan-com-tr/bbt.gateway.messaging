using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class OperatorReportInfo
    {
        public OperatorType OperatorType { get; set; }
        public bool isOtp { get; set; }
        public bool isFast { get; set; }
        public OperatorReportInfo AdditionalOperatorType { get; set; }
    }
}
