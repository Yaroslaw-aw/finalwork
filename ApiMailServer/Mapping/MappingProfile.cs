using ApiMailServer.Db;
using ApiMailServer.Dto;
using AutoMapper;

namespace ApiMailServer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MessageDto, Message>().ReverseMap();

            CreateMap<Message, MessagesSentDto>().ReverseMap();
        }
    }
}
