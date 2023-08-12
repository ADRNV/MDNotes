using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MdNotes.WebApi.Features.User
{
    public record SignInCommand(UserCore User) : IRequest<bool>; 

    public class CommandHandler : IRequestHandler<SignInCommand, bool>
    {
        private readonly SignInManager<UserEntity> _signInManager;

        private readonly UserManager<UserEntity> _userManager;

        private readonly IMapper _mapper;

        public CommandHandler(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager, IMapper mapper)
        {
            _signInManager = signInManager;

            _userManager = userManager;

            _mapper = mapper;
        }

        public async Task<bool> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<UserCore, UserEntity>(request.User);


            if (await _userManager.FindByEmailAsync(user.Email) is not null)
            {
                var signIn = await _signInManager.PasswordSignInAsync(user, request.User.Password, false, false);

                return signIn.Succeeded;
            }
            else
            {
                return false;
            }
        }
    }
}
