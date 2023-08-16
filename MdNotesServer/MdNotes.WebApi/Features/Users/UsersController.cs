using MdNotes.WebApi.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MdNotes.WebApi.Features.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{guid}")]
        public async Task<UserCore> GetUser([FromRoute]Guid guid)
        {
            return await _mediator.Send(new Get(guid));
        }

        [HttpPost("create")]
        public async Task<IdentityResult> CreateUser([FromBody]UserCore user)
        {
            return await _mediator.Send(new Create(user));
        }

        [HttpPatch("{guid}/update")]
        public async Task<bool> UpdateUser([FromRoute]Guid guid, [FromBody]UserCore user)
        {
            return await _mediator.Send(new Update(guid, user));
        }

        [HttpDelete("{guid}/delete")]
        public async Task<bool> UpdateUser([FromRoute] Guid guid)
        {
            return await _mediator.Send(new Delete(guid));
        }
    }
}
