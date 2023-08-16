using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MdNotes.WebApi.Features.Users.Commands
{
    public record Get(Guid Guid) : IRequest<UserCore>;

    public class GetHandler : IRequestHandler<Get, UserCore>
    {
        private readonly UserManager<UserEntity> _userManager;

        private readonly IMapper _mapper;

        public GetHandler(UserManager<UserEntity> userManager, IMapper mapper)
        {
            _userManager = userManager;
            
            _mapper = mapper;
        }

        public async Task<UserCore?> Handle(Get request, CancellationToken cancellationToken) =>
            _mapper.Map<UserEntity, UserCore>(await _userManager.FindByIdAsync(request.Guid.ToString()));
    }
}
