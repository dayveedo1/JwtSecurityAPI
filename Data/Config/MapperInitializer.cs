using AutoMapper;
using JwtSecurityApi.Data.Model;
using JwtSecurityApi.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtSecurityApi.Data.Config
{
    public class MapperInitializer: Profile
    {

        public MapperInitializer()
        {
            CreateMap<ApiUser, UserDto>().ReverseMap();
        }
        
    }
}
