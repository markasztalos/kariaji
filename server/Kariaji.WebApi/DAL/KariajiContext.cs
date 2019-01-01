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
            modelBuilder.Entity<Membership>()
                .HasKey(bc => new { bc.GroupId, MemberUserId = bc.UserId });
            modelBuilder.Entity<Membership>()
                .HasOne(bc => bc.Group)
                .WithMany(b => b.Memberships)
                .HasForeignKey(bc => bc.GroupId);
            modelBuilder.Entity<Membership>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.Memberships)
                .HasForeignKey(bc => bc.UserId);

            modelBuilder.WithDisabledCascadeDeleteOnForeignKeys();

            base.OnModelCreating(modelBuilder);

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Membership> Memberships { get; set; }
    }
}
