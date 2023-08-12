using AutoMapper;

namespace MdNotesServer.Infrastructure.MappingConfigurations
{
    public class UserEntityMappingConfiguration : Profile
    {
        public UserEntityMappingConfiguration()
        {
            CreateMap<UserEntity, UserCore>()
                .ForMember(u => u.Name, o => o.MapFrom(u => u.UserName));
        }
    }
}
