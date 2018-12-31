using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Kariaji.WebApi.DAL
{
    public class KariajiContext : DbContext
    {
        public KariajiContext(DbContextOptions<KariajiContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupAdministrator>()
                .HasKey(bc => new { bc.GroupId, bc.AdministratorId });
            modelBuilder.Entity<GroupAdministrator>()
                .HasOne(bc => bc.Group)
                .WithMany(b => b.GroupAdministrators)
                .HasForeignKey(bc => bc.GroupId);
            modelBuilder.Entity<GroupAdministrator>()
                .HasOne(bc => bc.Administrator)
                .WithMany(c => c.GroupAdministrators)
                .HasForeignKey(bc => bc.AdministratorId);

            modelBuilder.WithDisabledCascadeDeleteOnForeignKeys();

            base.OnModelCreating(modelBuilder);

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupAdministrator> GroupAdministrators { get; set; }
    }
}
