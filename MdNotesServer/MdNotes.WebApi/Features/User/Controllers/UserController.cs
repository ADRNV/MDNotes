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
        public void SignIn([FromBody]UserCore user)
        {
            _mediator.Send(new SignInCommand(user));
        }
    }
}
