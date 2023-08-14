using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;

namespace MdNotes.WebApi.Features.User
{
    public record RegisterCommand(UserCore User) : IRequest<bool>;

    public class RegisterHandler : IRequestHandler<RegisterCommand, bool>
    {
        private readonly UserManager<UserEntity> _userManager;

        private readonly SignInManager<UserEntity> _signInManager;

        private readonly IPasswordHasher<UserEntity> _passwordHasher;

        private readonly IMapper _mapper;

        public RegisterHandler(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, IPasswordHasher<UserEntity> passwordHasher, IMapper mapper)
        {

            _signInManager = signInManager;

            _userManager = userManager;

            _passwordHasher = passwordHasher;

            _mapper = mapper;

        }

        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.User.Email);

            if (user is null)
            {
                user = _mapper.Map<UserCore, UserEntity>(request.User);

                user.PasswordHash = _passwordHasher.HashPassword(user, request.User.Password);

                await _userManager.CreateAsync(user);

                await _signInManager.SignInAsync(user, true);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
