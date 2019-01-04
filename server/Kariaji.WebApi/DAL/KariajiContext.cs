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

            modelBuilder.Entity<IdeaUser>()
                .HasKey(bc => new { bc.UserId, bc.IdeaId });
            modelBuilder.Entity<IdeaUser>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.Ideas)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<IdeaUser>()
                .HasOne(bc => bc.Idea)
                .WithMany(c => c.Users)
                .HasForeignKey(bc => bc.IdeaId);

            
            modelBuilder.Entity<IdeaTargetGroup>()
                .HasKey(bc => new { bc.GroupId, bc.IdeaId });
            modelBuilder.Entity<IdeaTargetGroup>()
                .HasOne(bc => bc.Group)
                .WithMany(b => b.IdeaTargets)
                .HasForeignKey(bc => bc.GroupId);
            modelBuilder.Entity<IdeaTargetGroup>()
                .HasOne(bc => bc.Idea)
                .WithMany(c => c.TargetGroups)
                .HasForeignKey(bc => bc.IdeaId);

            modelBuilder.Entity<Idea>()
                .HasOne(i => i.Reservation)
                .WithOne(r => r.Idea)
                .HasForeignKey<Reservation>(r => r.IdeaId);


            modelBuilder.WithDisabledCascadeDeleteOnForeignKeys();

            base.OnModelCreating(modelBuilder);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<UserGroupInvitation> Invitations { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Idea> Ideas { get; set; }
        public DbSet<IdeaTargetGroup> IdeaTargetGroups { get; set; }
        public DbSet<IdeaUser> IdeaUsers { get; set; }
        public DbSet<IdeaComment> IdeaComments { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationJoin> ReservationJoins { get; set; }


    }
}

