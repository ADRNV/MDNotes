using MdNotesServer.Infrastructure.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MdNotes.WebApi.Features.User.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;    
        }

        [HttpPost("sign-in")]
        public async Task<JwtAuthResult> SignIn([FromBody]UserCore user)
        {
            return await _mediator.Send(new SignInCommand(user));
        }

        [HttpPost("register")]
        public async Task<JwtAuthResult> Register([FromBody] UserCore user)
        {
            return await _mediator.Send(new RegisterCommand(user)); 
        }
    }
}
