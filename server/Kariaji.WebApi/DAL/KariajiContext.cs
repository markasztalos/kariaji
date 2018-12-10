using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Kariaji.WebApi.DAL
{
    public class KariajiContext : DbContext
    {
        public KariajiContext(DbContextOptions<KariajiContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get;set; }
        public DbSet<Configuration> Configurations { get;set; }

    }
}
