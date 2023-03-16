using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class TransactionsDto
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public int Count { get; set; }
    }
}
