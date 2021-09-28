using JwtSecurityApi.Data.Config;
using JwtSecurityApi.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtSecurityApi.Data
{
    public class JwtSecurityDbContext: IdentityDbContext<ApiUser>
    {
        public JwtSecurityDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new RoleConfiguration()); 
        }

    }
}
