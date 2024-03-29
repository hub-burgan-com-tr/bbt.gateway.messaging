﻿using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace bbt.gateway.common.Repositories
{
    public class DirectBlacklistRepository : SmsBankingRepository<SmsDirectBlacklist>, IDirectBlacklistRepository
    {
        public DirectBlacklistRepository(SmsBankingDatabaseContext context) : base(context)
        { 
            
        }

        public async Task<SmsDirectBlacklist> GetLastBlacklistEntry(string number)
        {
            return await Context.SmsDirectBlackList.Where(b => b.GsmNumber == number)
                .OrderByDescending(b => b.RecordDate).FirstOrDefaultAsync();
        }

    }
}
