using bbt.gateway.common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace bbt.gateway.common
{
    public class DodgeDatabaseContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<UserDevice> UserDevice { get; set; }
        public DodgeDatabaseContext(DbContextOptions<DodgeDatabaseContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
        }

    }
}
