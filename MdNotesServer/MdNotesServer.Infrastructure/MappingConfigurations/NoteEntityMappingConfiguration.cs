using AutoMapper;

namespace MdNotesServer.Infrastructure.MappingConfigurations
{
    public class NoteEntityMappingConfiguration : Profile
    {
        public NoteEntityMappingConfiguration()
        {
            CreateMap<NoteEntity, NoteCore>()
                .ReverseMap()
                .ForMember(n => n.User, o => o.Ignore());
        }
    }
}
