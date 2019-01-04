using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace Kariaji.WebApi.Helpers
{
    public static class DbContextHelper
    {
        public static IMutableForeignKey WithDisabledCascadeDelete(this IMutableForeignKey fk)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
            return fk;
        }

        public static ModelBuilder WithDisabledCascadeDeleteOnForeignKeys(this ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.WithDisabledCascadeDelete();
            return modelBuilder;
        }

        public static async Task<KariajiContext> InitializeDemoData(this KariajiContext ctx, IServiceScope serviceScope)
        {
            var protectionSvc = serviceScope.ServiceProvider.GetRequiredService<ProtectionService>();
            var mark = new User
            {
                DisplayName = "Márk",
                Email = "markasztalos@gmail.com",
                Password = protectionSvc.HashPassword("mark")
            };
            var mari = new User
            {

                DisplayName = "Mari",
                Email = "homaro@kariaji.com",
                Password = protectionSvc.HashPassword("mari")
            };
            var julcsi = new User
            {

                DisplayName = "Julcsi",
                Email = "julcsi@kariaji.com",
                Password = protectionSvc.HashPassword("julcsi")
            };
            var palko = new User
            {

                DisplayName = "Palkó",
                Email = "palko@kariaji.com",
                Password = protectionSvc.HashPassword("palko")
            };
            var mami = new User
            {

                DisplayName = "Mami",
                Email = "mami@kariaji.com",
                Password = protectionSvc.HashPassword("mami")
            };
            var apo = new User
            {

                DisplayName = "Apó",
                Email = "apo@kariaji.com",
                Password = protectionSvc.HashPassword("apo")
            };

            var gergo = new User
            {

                DisplayName = "Gergő",
                Email = "gergo@kariaji.com",
                Password = protectionSvc.HashPassword("gergo")
            };
            var andi = new User
            {

                DisplayName = "Andi",
                Email = "andi@kariaji.com",
                Password = protectionSvc.HashPassword("andi")
            };

            var users = new[] { mark, mari, julcsi, palko, mami, apo, gergo, andi };
            foreach (var user in users)
                ctx.Users.Add(user);

            var hodsagi = new Group
            {
                CreationDate = DateTime.Now,
                CreatorUser = mari,
                DisplayName = "Hódsági család",
            };
            var asztalos = new Group
            {
                CreationDate = DateTime.Now,
                CreatorUser = mark,
                DisplayName = "Asztalos család"
            };

            var groups = new[] { hodsagi, asztalos };
            foreach (var group in groups)
                ctx.Groups.Add(group);

            void addMember(Group g, params User[] usrs)
            {
                foreach (var user in usrs)
                    ctx.Memberships.Add(new Membership
                    {
                        Group = g,
                        User = user,
                        IsAdministrator = user == mari || user == mark

                    });
            }
            addMember(hodsagi, mari, mark, julcsi, palko, mami, apo);
            addMember(asztalos, mari, mark, gergo, andi);

            var x = 0;
            Idea WriteIdea(User cr, string text, IEnumerable<Group> gs, IEnumerable<User> usrs)
            {
                x++;



                var idea = new Idea
                {
                    CreatorUser = cr,
                    CreationTime = DateTime.Now,
                    TextDelta = $@"{{ ""ops"": [{{ ""insert"": ""{text}"" }}] }}"
                };
                idea.TargetGroups = gs.Select(g => new IdeaTargetGroup
                {
                    Idea = idea,
                    Group = g
                }).ToList();
                idea.Users = usrs.Select(u => new IdeaUser
                {
                    User = u,
                    Idea = idea
                }).ToList();
                ctx.Ideas.Add(idea);
                if (x % 5 == 0)
                {
                    idea.Comments = new List<IdeaComment>();
                    idea.Comments.Add(new IdeaComment
                    {
                        Idea = idea,
                        CreationTime = DateTime.Now,
                        TextDelta = $@"{{ ""ops"": [{{ ""insert"": ""Komment1"" }}] }}",
                        User = mark
                    });
                    idea.Comments.Add(new IdeaComment
                    {
                        Idea = idea,
                        CreationTime = DateTime.Now,
                        TextDelta = $@"{{ ""ops"": [{{ ""insert"": ""Komment2"" }}] }}",
                        User = julcsi
                    });
                }

                return idea;
            }

            for (int i = 0; i < 30; i++)
            {
                WriteIdea(mari, $"Ötlet{i++}", new[] { asztalos, hodsagi }, new[] { mark });
                WriteIdea(mari, $"Ötlet{i++}", new[] { asztalos, hodsagi }, new[] { mari });

                WriteIdea(mari, $"Ötlet{i++}", new[] { asztalos, hodsagi }, new[] { julcsi, palko, mami, apo });
                WriteIdea(julcsi, $"Ötlet{i++}", new[] { hodsagi }, new[] { mari });
                WriteIdea(julcsi, $"Ötlet{i++}", new[] { hodsagi }, new[] { julcsi });
                WriteIdea(julcsi, $"Ötlet{i++}", new[] { hodsagi }, new[] { palko });
                WriteIdea(julcsi, $"Ötlet{i++}", new[] { hodsagi }, new[] { julcsi, palko });
            }

            await ctx.SaveChangesAsync();
            return ctx;
        }


    }


}
