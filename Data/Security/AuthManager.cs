using JwtSecurityApi.Data.Model;
using JwtSecurityApi.Data.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtSecurityApi.Data.Security
{
    public class AuthManager : IAuthManager
    {

        private readonly UserManager<ApiUser> userManager;
        private readonly IConfiguration configuration;
        private ApiUser user;

        public AuthManager(UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        //creating a token
        public async Task<string> CreateToken()
        {
            //fetch the signing credentials
            var signingCredentials = GetSigningCredentials();
            //fetch the claims/Authorities
            var claims = await GetClaims();
            //generate a token
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public async Task<bool> ValidateUser(LoginUserDto userDto)
        {
            //fetch the user we want to validate by username
            user = await userManager.FindByNameAsync(userDto.Email);
            //it return the user & validate the password
            return (user != null && await userManager.CheckPasswordAsync(user, userDto.Password));
        }

        private  SigningCredentials GetSigningCredentials()
        {
            try
            {
                //fetch the key we created and saved as a local variable
                var key = Environment.GetEnvironmentVariable("SKEY");
                //encrypting the key we just fetched
                var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256Signature);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("lifetime").Value));

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("Issuer").Value,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
             );

            return token;
        }
    }
}
