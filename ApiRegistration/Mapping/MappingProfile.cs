using ApiRegistration.Db;
using ApiRegistration.Dto;
using AutoMapper;

namespace ApiRegistration.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, GetUsersDto>().ReverseMap();
        }
    }
}
