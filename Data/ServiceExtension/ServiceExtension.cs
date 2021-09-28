using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtSecurityApi.Data.ServiceExtension
{
    public static class ServiceExtension
    {
        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSetting = configuration.GetSection("Jwt");
            var key = Environment.GetEnvironmentVariable("SKEY");

            services.AddAuthentication(o =>
            {
                //adding Authentication to the application, which i'm setting the default to JWT
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //check for whatever request the application recieves & verify it is valid
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(o =>
             {
                 o.TokenValidationParameters = new TokenValidationParameters
                 {
                     //validate the issuer which we set in the appsetting.json
                     ValidateIssuer = true,
                     ValidateAudience = false,
                     ValidateActor = false,
                     //validate the Lifesan duration of the token
                     ValidateLifetime = true,
                     //validate the signing key which we created
                     ValidateIssuerSigningKey = true,
                     //the valid issuer is Issuer gotten from the appsetting.json
                     ValidIssuer = jwtSetting.GetSection("Issuer").Value,
                     //Encrypting the issuer signing key
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                 };
             });
        }
    }
}
