using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bbt.gateway.common.Repositories
{
    public class MailConfigurationRepository : Repository<MailConfiguration>, IMailConfigurationRepository
    {
        public MailConfigurationRepository(DatabaseContext context) : base(context)
        { 
        
        }
       
    }
}
