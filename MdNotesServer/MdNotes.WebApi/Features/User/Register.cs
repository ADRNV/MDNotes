using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MdNotes.WebApi.Features.User
{
    public record RegisterCommand(UserCore User) : IRequest<bool>;

    public class RegisterHandler : IRequestHandler<RegisterCommand, bool>
    {
        private readonly SignInManager<UserEntity> _signInManager;

        private readonly IPasswordHasher<UserEntity> _passwordHasher;

        private readonly IMapper _mapper;

        public RegisterHandler(SignInManager<UserEntity> signInManager, IPasswordHasher<UserEntity> passwordHasher, IMapper mapper)
        {

            _signInManager = signInManager;

            _passwordHasher = passwordHasher;

            _mapper = mapper;

        }

        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(request.User.Email);


            if (user is null)
            {
                user = _mapper.Map<UserCore, UserEntity>(request.User);

                user.PasswordHash = _passwordHasher.HashPassword(user, request.User.Password);

                var userCreated = await _signInManager.UserManager.CreateAsync(user);

                await _signInManager.SignInAsync(user, false);

                return userCreated.Succeeded;
            }
            else
            {
                return false;
            }
        }
    }
}
