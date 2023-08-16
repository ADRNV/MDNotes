using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MdNotes.WebApi.Features.Users.Commands
{
    public record Create(UserCore User) : IRequest<IdentityResult>;

    public class CreateHandler : IRequestHandler<Create, IdentityResult>
    {
        private readonly UserManager<UserEntity> _userManager;

        private readonly IMapper _mapper;

        public CreateHandler(UserManager<UserEntity> userManager, IMapper mapper)
        {

            _userManager = userManager;

            _mapper = mapper;

        }

        public async Task<IdentityResult> Handle(Create request, CancellationToken cancellationToken)
        {
            if(_userManager.FindByEmailAsync(request.User.Email) is null)
            {
                var identityUser = _mapper.Map<UserCore, UserEntity>(request.User);

                return await _userManager.CreateAsync(identityUser);
            }
            else
            {
                throw new InvalidOperationException("User are exists");
            }
        }
    }
}
