using AutoMapper;
using MdNotesServer.Infrastructure.Entities;
using MdNotesServer.Infrastructure.Jwt;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MdNotes.WebApi.Features.User
{
    public record RegisterCommand(UserCore User) : IRequest<JwtAuthResult>;

    public class RegisterHandler : IRequestHandler<RegisterCommand, JwtAuthResult>
    {
        private readonly SignInManager<UserEntity> _signInManager;

        private readonly IPasswordHasher<UserEntity> _passwordHasher;

        private readonly IJwtAuthManager<UserEntity> _jwtAuthManager;

        private readonly IMapper _mapper;

        public RegisterHandler(SignInManager<UserEntity> signInManager, IJwtAuthManager<UserEntity> jwtAuthManager, IPasswordHasher<UserEntity> passwordHasher, IMapper mapper)
        {

            _signInManager = signInManager;

            _passwordHasher = passwordHasher;

            _jwtAuthManager = jwtAuthManager;

            _mapper = mapper;

        }

        public async Task<JwtAuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(request.User.Email);

            if (user is null)
            {
                user = _mapper.Map<UserCore, UserEntity>(request.User);

                user.PasswordHash = _passwordHasher.HashPassword(user, request.User.Password);

                await _signInManager.UserManager.CreateAsync(user);

                var createdUser = await _signInManager.UserManager.FindByEmailAsync(user.Email);

                await _signInManager.UserManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                await _signInManager.UserManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "User"));

                await _signInManager.SignInAsync(user, false);

                return await _jwtAuthManager.GenerateTokens(user, await _signInManager.UserManager.GetClaimsAsync(user), DateTime.Now);
            }
            else
            {
                throw new InvalidOperationException("User is exists");
            }
        }
    }
}
