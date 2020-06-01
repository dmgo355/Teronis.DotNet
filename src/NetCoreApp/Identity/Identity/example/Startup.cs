﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Teronis.Identity.Controllers;
using Teronis.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Teronis.Identity.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Teronis.Identity
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
            services.AddMvc()
                .AddIdentityControllers();

            services.AddDbContext<BearerIdentityDbContext>(options => {
                options.UseSqlite("Data Source=bearerIdentity.db;");

                // Uncomment when using EntityFrameworkCore.InMemory.
                //options.ConfigureWarnings(warnings =>
                //    warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("a security key a security key a security key a security key"));

            services.AddIdentity<UserEntity, RoleEntity>()
                .AddEntityFrameworkStores<BearerIdentityDbContext>()
                .AddAccountManager<BearerIdentityDbContext>()
                .AddBearerTokenStore<BearerIdentityDbContext>()
                .AddBearerSignInManager<BearerIdentityDbContext>(options => {
                    options.IncludeErrorDetails = true;

                    var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    options.CreateDefaultedTokenDescriptor = () => new BearerTokenDescriptor(signingCredentials);
                });

            services.AddAuthentication()
                .AddIdentityBasic<UserEntity>()
                .AddIdentityJwtRefreshToken(new JwtBearerAuthenticationOptions(securityKey))
                .AddJwtAccessToken(new JwtBearerAuthenticationOptions(securityKey) {
                    IncludeErrorDetails = true
                });

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.Map("/", (context) => context.Response.WriteAsync("Hello Teronis.Identity"));
            });
        }
    }
}
