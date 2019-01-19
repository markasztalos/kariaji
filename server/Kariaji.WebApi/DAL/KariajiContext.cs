using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Kariaji.WebApi.DAL
{
    public class KariajiContext : DbContext
    {
        public KariajiContext(DbContextOptions<KariajiContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Membership>()
            //    .HasKey(bc => new { bc.GroupId, MemberUserId = bc.UserId });
            //modelBuilder.Entity<Membership>()
            //    .HasOne(bc => bc.Group)
            //    .WithMany(b => b.Memberships)
            //    .HasForeignKey(bc => bc.GroupId);
            //modelBuilder.Entity<Membership>()
            //    .HasOne(bc => bc.User)
            //    .WithMany(c => c.Memberships)
            //    .HasForeignKey(bc => bc.UserId);

            //modelBuilder.Entity<IdeaUser>()
            //    .HasKey(bc => new { bc.UserId, bc.IdeaId });
            //modelBuilder.Entity<IdeaUser>()
            //    .HasOne(bc => bc.User)
            //    .WithMany(b => b.Ideas)
            //    .HasForeignKey(bc => bc.UserId);
            //modelBuilder.Entity<IdeaUser>()
            //    .HasOne(bc => bc.Idea)
            //    .WithMany(c => c.Users)
            //    .HasForeignKey(bc => bc.IdeaId);


            //modelBuilder.Entity<IdeaTargetGroup>()
            //    .HasKey(bc => new { bc.GroupId, bc.IdeaId });
            //modelBuilder.Entity<IdeaTargetGroup>()
            //    .HasOne(bc => bc.Group)
            //    .WithMany(b => b.IdeaTargets)
            //    .HasForeignKey(bc => bc.GroupId);
            //modelBuilder.Entity<IdeaTargetGroup>()
            //    .HasOne(bc => bc.Idea)
            //    .WithMany(c => c.TargetGroups)
            //    .HasForeignKey(bc => bc.IdeaId);

            //modelBuilder.Entity<Idea>()
            //    .HasOne(i => i.Reservation)
            //    .WithOne(r => r.Idea)
            //    .HasForeignKey<Reservation>(r => r.IdeaId);


            //modelBuilder.Entity<ReservationJoin>()
            //    .HasKey(j => new { j.ReservationId, j.UserId });

            //modelBuilder.Entity<Reservation>()
            //    .HasOne(res => res.Idea)
            //    .WithOne(idea => idea.Reservation);
            //modelBuilder.Entity<Idea>()
            //    .HasOne(i => i.Reservation)
            //    .WithOne(r => r.Idea)
            //    .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.IdeaId);
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.ReserverUserId);
            modelBuilder.Entity<Group>()
                .HasIndex(u => u.CreatorUserId);


            modelBuilder.Entity<ReservationJoin>()
                .HasIndex(rj => new {rj.ReservationId, rj.UserId})
                .IsUnique();
            modelBuilder.Entity<Membership>()
                .HasIndex(m => new {m.GroupId, m.UserId})
                .IsUnique();
            modelBuilder.Entity<Membership>()
                .HasIndex(m => m.UserId);


            modelBuilder.Entity<Membership>()
                .HasIndex(m => m.GroupId);

            modelBuilder.Entity<Avatar>()
                .HasIndex(m => m.UserId)
                .IsUnique();

            modelBuilder.Entity<UserGroupInvitation>()
                .HasIndex(m => m.GroupId);

            modelBuilder.Entity<IdeaTargetGroup>()
                .HasIndex(itg => new {itg.GroupId, itg.IdeaId})
                .IsUnique();
            modelBuilder.Entity<IdeaTargetGroup>()
                .HasIndex(i => i.GroupId);

            modelBuilder.Entity<IdeaUser>()
                .HasIndex(iu => new {iu.IdeaId, iu.UserId})
                .IsUnique();
            modelBuilder.Entity<IdeaUser>()
                .HasIndex(iu => iu.IdeaId);
            modelBuilder.Entity<IdeaUser>()
                .HasIndex(iu => iu.UserId);

            modelBuilder.Entity<Idea>()
                .HasIndex(i => i.CreatorUserId);
            modelBuilder.Entity<IdeaComment>()
                .HasIndex(i => i.IdeaId);
            modelBuilder.Entity<IdeaComment>()
                .HasIndex(i => i.UserId);
            
                
            



            modelBuilder.WithDisabledCascadeDeleteOnForeignKeys();

            base.OnModelCreating(modelBuilder);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory( new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) }))
                .EnableSensitiveDataLogging();
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

