using JwtSecurityApi.Data;
using JwtSecurityApi.Data.Config;
using JwtSecurityApi.Data.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtSecurityApi
{
    public class Startup
    {
        private static readonly string AllowOrigins = "AllowAll";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<JwtSecurityDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("JwtSecurityAPI"))
           );

            //IdentityCore config
            var builder = services.AddIdentityCore<ApiUser>(x => x.User.RequireUniqueEmail = true);
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<JwtSecurityDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication();

            services.AddCors(cs => {
                cs.AddPolicy(AllowOrigins,
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddAutoMapper(typeof(MapperInitializer));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                { Title = "JwtSecurityAPI", Version = "v1" });
            });

            services.AddControllers();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }

            app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JwtSecurityAPI v1"));
            app.UseSwaggerUI(c => {
                //c.DefaultModelRendering(ModelRendering.Example);
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "JwtSecurity");
                //c.RoutePrefix = string.Empty;
                string swaggerJsonPath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonPath}/swagger/v1/swagger.json", "JwtSecurityAPI");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowOrigins");

            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");


                endpoints.MapControllers();
            });
        }
    }
}
