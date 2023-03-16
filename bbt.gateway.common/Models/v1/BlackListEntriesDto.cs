using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.gateway.common.Models
{
    public class BlackListEntriesDto
    {
        public IEnumerable<BlackListEntry> BlackListEntries { get; set; }
        public int Count { get; set; }
    }
}
