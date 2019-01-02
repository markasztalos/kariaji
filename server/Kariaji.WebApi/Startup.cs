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
            var connection = @"Server=(localdb)\kariaji;Database=kariajiBeta;Trusted_Connection=True;ConnectRetryCount=0";

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
            }
        }
    }


}
