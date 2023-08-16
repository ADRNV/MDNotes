using AutoMapper;
using MdNotesServer.Infrastructure.Entities;
using MdNotesServer.Infrastructure.Jwt;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MdNotes.WebApi.Features.User
{
    public record SignInCommand(UserCore User) : IRequest<JwtAuthResult>; 

    public class SignInHandler : IRequestHandler<SignInCommand, JwtAuthResult>
    {
        private readonly SignInManager<UserEntity> _signInManager;

        private readonly IJwtAuthManager<UserEntity> _jwtAuthManager;

        private readonly IMapper _mapper;

        public SignInHandler(SignInManager<UserEntity> signInManager, IJwtAuthManager<UserEntity> jwtAuthManager, IMapper mapper)
        {
            _signInManager = signInManager;

            _jwtAuthManager = jwtAuthManager;

            _mapper = mapper;
        }

        public async Task<JwtAuthResult> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(request.User.Email);

            var signeWithPassword = false;

            if (user is not null & await _signInManager.CanSignInAsync(user))
            {
                signeWithPassword = (await _signInManager.CheckPasswordSignInAsync(user, request.User.Password, false)).Succeeded;
            }
            else
            {
                throw new InvalidOperationException("User blocked");
            }
            if (signeWithPassword)
            {
                var userClaims = await _signInManager.UserManager.GetClaimsAsync(user);

                return await _jwtAuthManager.GenerateTokens(user,
                    userClaims,
                    DateTime.Now);
            }
            else
            {
                throw new InvalidOperationException("Wrong password");
            }
        }
    }
}
