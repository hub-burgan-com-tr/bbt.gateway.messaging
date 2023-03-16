using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace bbt.gateway.common.Repositories
{
    public class MailRequestLogRepository : Repository<MailRequestLog>, IMailRequestLogRepository
    {
        public MailRequestLogRepository(DatabaseContext context) : base(context)
        { 
        
        }

    }
}
