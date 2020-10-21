using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace KeycloakDemo
{
    public class Startup
    {


        public Startup(IWebHostEnvironment env, IConfiguration config)
        {
            this.Configuration = config;
            this.Environment = env;
        }

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddIdentityServerAuthentication(options =>
             {
                 options.Authority = "http://localhost:8080/auth/realms/master";
                 options.ApiName = "test";
                 options.RequireHttpsMetadata = false;
             });




            #region JwtBearer
            //services.AddAuthentication(options => 
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;                
            //})
            //    .AddJwtBearer(options =>
            //    {
            //        options.Authority = "http://localhost:8080/auth/realms/master";
            //        options.Audience = "test";
            //        options.MetadataAddress = "http://localhost:8080/auth/realms/master/.well-known/openid-configuration/";


            //        options.RequireHttpsMetadata = false;
            //        options.Events = new JwtBearerEvents()
            //        {
            //            OnAuthenticationFailed = c =>
            //            {
            //                c.NoResult();

            //                c.Response.StatusCode = 500;
            //                c.Response.ContentType = "text/plain";
            //                if (Environment.IsDevelopment())
            //                {
            //                    return c.Response.WriteAsync(c.Exception.ToString());
            //                }
            //                return c.Response.WriteAsync("An error occured processing your authentication.");
            //            }
            //        };
            //    });
            #endregion

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", policy => policy.RequireRole("administrator"));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }            

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
