
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kariaji.WebApi.DAL;
using Kariaji.WebApi.Helpers;
using Kariaji.WebApi.Middlewares;
using Kariaji.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using ILogger = NLog.ILogger;

namespace Kariaji.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("KariajiDatabase");

            services.AddDbContext<KariajiContext>
                (options => options.UseSqlServer(connection));

            services.AddAuthentication(o =>
               {
                   o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               })
               .AddJwtBearer(cfg =>
               {
                   var optionsBuilder = new DbContextOptionsBuilder<KariajiContext>();
                   optionsBuilder.UseSqlServer(connection);
                   string jwtString = null;
                   byte[] jwtKey = null;
                   using (var ctx = new KariajiContext(optionsBuilder.Options))
                   {
                       jwtString =
                           (ctx.Database.GetService<IRelationalDatabaseCreator>()
                               .Exists()
                               ? ctx.Configurations.First().JWTKey
                               : ProtectionService.JwtKey);
                   }

                   jwtKey = Convert.FromBase64String(jwtString);
                   //cfg.RequireHttpsMetadata = false;
                   cfg.SaveToken = true;
                   cfg.TokenValidationParameters = new TokenValidationParameters()
                   {
                       //IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                       //ValidAudience = "Users",
                       //ValidateIssuerSigningKey = true,
                       //ValidateLifetime = true,
                       //ValidIssuer = "Kariaji"
                       ClockSkew = TimeSpan.FromMinutes(1),
                       ValidIssuer = "Kariaji",
                       ValidAudience = "Users",
                       IssuerSigningKey = new SymmetricSecurityKey(jwtKey),

                       //ValidIssuer = Configuration["Tokens:Issuer"],
                       //ValidAudience = Configuration["Tokens:Issuer"],
                       //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                   };

               });

            services.AddCors();

            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                ;





            services.AddScoped(typeof(AuthenticationService));

            services.AddDataProtection();

            services.AddScoped(typeof(MailingService));
            services.AddScoped(typeof(ConfigurationProviderService));
            services.AddScoped(typeof(ProtectionService));
            services.AddScoped(typeof(UserGroupManagerService));
            services.AddScoped(typeof(IdeasManagerService));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !System.IO.Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });
            app
                .UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string> { "index.html" } })
                .UseStaticFiles();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();


            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var defaultConfigurationProviderService = serviceScope.ServiceProvider.GetRequiredService<ConfigurationProviderService>();
                var context = serviceScope.ServiceProvider.GetRequiredService<KariajiContext>();
                context.Database.EnsureCreated();
                if (!context.Configurations.Any())
                {
                    context.Configurations.Add(defaultConfigurationProviderService.DefaultConfiguration);
                    context.SaveChanges();
                    //demo data
                    context.InitializeDemoData(serviceScope).Wait();



                }
                Test(context);
            }
        }

        public void Test(KariajiContext ctx)
        {
            //var x = ctx.Ideas.ToList();

            //var query = ctx.Ideas
            //        .Include(i => i.Reservation).ThenInclude(r => r.Joins)
            //        .Include(i => i.TargetGroups).Include(i => i.Users)
            //        .Include(i => i.Users)
            //        .Include(i => i.Comments)
            //    ;
            //.Where(i =>
            //    (i.CreatorUserId == userId ||
            //     (i.TargetGroups.Any(g => g.Group.Memberships.Any(m => m.UserId == userId && !m.IsDeleted)) &&
            //      i.Users.All(u => u.UserId != userId))) &&
            //    (filteredTargetGroupIds == null || filteredTargetGroupIds.Any(g => i.TargetGroups.Any(g2 => g2.GroupId == g))) &&
            //    (filteredTargetUserIds == null || filteredTargetUserIds.Any(g => i.Users.Where(u => !u.IsSecret).Any(u => u.UserId == g))) &&
            //    (!onlyNotReserved || (i.ReservationId == null || i.Reservation.ReserverUserId == userId))  &&
            //    (!onlyReservedByMe || (i.ReservationId != null) && i.Reservation.ReserverUserId == userId) &&
            //    (!onlySentByMe || i.CreatorUserId == userId)
            //)

            //.OrderByDescending(i => i.CreationTime);
            //var result = query.ToList();




            //var r3 = ctx.Ideas
            //    .Include(i => i.Reservation) //.ThenInclude(r => r.ReservationJoins)
            //    //.Include(i => i.TargetGroups)
            //    //.Include(i => i.Users)
            //    .Join(ctx.IdeaTargetGroups, idea => idea.Id, itg => itg.IdeaId,
            //        (i, itg) => new {Idea = i, TargetGroup = itg})
            //    .Join(ctx.IdeaUsers, item => item.Idea.Id, iu => iu.IdeaId,
            //        (item, iu) => new {item.Idea, item.TargetGroup, item.Idea.Reservation, User = iu})
            //    .GroupJoin(ctx.ReservationJoins.Where(j => j.UserId == 5), item => item.Reservation.Id, j => j.ReservationId, (item, joins) => new
            //    {
            //        item.Idea,
            //        item.TargetGroup,
            //        item.Reservation,
            //        item.User,
            //        Joins = joins
            //    })
            //    .ToList().Select(item => item.Idea).ToList();
            //var r4 = r3.Count;

            //var ideas = ctx.Ideas
            //    .Include(i => i.Reservation)
            //    .GroupJoin(ctx.IdeaTargetGroups, idea => idea.Id, itg => itg.IdeaId, (i, itgs) => new { Idea = i, Reservation=i.Reservation, TargetGroups = itgs })
            //    .GroupJoin(ctx.IdeaUsers, item => item.Idea.Id, u => u.IdeaId, (item, us) => new { item.Idea, item.TargetGroups, item.Reservation, Users = us })
            //    //.GroupJoin(ctx.ReservationJoins.Where(j => j.UserId > 0), item => item.Idea.Reservation.Id, j => j.ReservationId, (item, joins) => new { item.Idea, item.TargetGroups, item.Users, Joins = joins, item.Reservation })
            //    .ToList();
            //var x = 5;

            //var x = ctx.Ideas.Select(i => new
            //{
            //    Idea = i,
            //    TargetGroups = i.TargetGroups
            //}).ToList();

            //var r5 = r3.Count + 1;

            //var x = ctx.Ideas.Include(i => i.Reservation).ThenInclude(r => r.Joins).ToList();
            //var x = ctx.Reservations.Include(r => r.ReservationJoins).ToList();

            //Console.WriteLine(r3.First().Idea.TargetGroups.First().GroupId);
        }
    }




}
