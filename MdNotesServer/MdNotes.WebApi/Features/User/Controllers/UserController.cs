using MdNotesServer.Infrastructure.Entities;
using MdNotesServer.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MdNotes.WebApi.Features.User.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = AuthorizeConstants.Policies.User, Roles = AuthorizeConstants.Roles.User)]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;    
        }

        [HttpPost("sign-in")]
        [AllowAnonymous]
        public async Task<JwtAuthResult> SignIn([FromBody]UserCore user)
        {
            return await _mediator.Send(new SignInCommand(user));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<JwtAuthResult> Register([FromBody] UserCore user)
        {
            return await _mediator.Send(new RegisterCommand(user)); 
        }
    }
}
