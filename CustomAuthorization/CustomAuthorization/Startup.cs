using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomActionFilter.Context;
using CustomAuthorization.ActionFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetCoreJWTAuth.App;

namespace CustomActionFilter
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = "yourdomain.com",
                      ValidAudience = "yourdomain.com",
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))
                  };
              });
            List<string> l = new List<string>();
            l.Add("Admin");
            l.Add("User");
            l.Add("Super User");


            //Configure for in memory database
            services.AddDbContext<ApplicationDbContext>(context => { context.UseInMemoryDatabase("AcademyDB"); });

            services.AddAuthorization(options =>
            {
               // options.AddPolicy("CustomRole", policy => policy.AddRequirements(new PolicyBasedAuthorization.CustomRoleRequirement()));
                options.AddPolicy("TrainedStaffOnly", policy => policy.RequireClaim("CompletedBasicTraining"));
                //options.AddPolicy("CustomRole", policy => policy.RequireClaim(CustomClaimType.Role).AddRequirements(new CustomRoleRequirement ("admin")));
                options.AddPolicy("CustomRole", policy => policy.Requirements.Add(new CustomRoleRequirement(l)));
                //.AddRequirements(new MinimumMonthsEmployedRequirement(3)));
        });

            services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, CustomRoleHandler>();
            services.AddMvc();

            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMvcWithDefaultRoute();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
