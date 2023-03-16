using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace bbt.gateway.common.Repositories
{
    public class MailResponseLogRepository : Repository<MailResponseLog>, IMailResponseLogRepository
    {
        public MailResponseLogRepository(DatabaseContext context) : base(context)
        { 
            
        }

    }
}
