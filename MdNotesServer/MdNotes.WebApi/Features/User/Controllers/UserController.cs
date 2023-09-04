using MdNotes.WebApi.Features.User.Commands;
using MdNotesServer.Infrastructure.Entities;
using MdNotesServer.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("notes")]
        public async Task<IEnumerable<NoteCore>> GetUserNotesAsync([FromQuery] int pageSize = 3,[FromQuery] int page = 1) =>
           await _mediator.Send(new GetUserNotesCommand(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), pageSize, page));

        [HttpPost("note/create")]
        public async Task<Guid> CreateUserNote([FromBody]NoteCore note) =>
            await _mediator.Send(new CreateNoteCommand(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), note));

        [HttpPatch("note/{noteId}/update")]
        public async Task<NoteCore> UpdateUserNote([FromRoute] Guid noteId, [FromBody] NoteCore note) =>
            await _mediator.Send(new UpdateNoteCommand(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), noteId, note));

        [HttpPatch("note/{noteId}/delete")]
        public async Task<bool> DeleteUserNote([FromRoute] Guid noteId) =>
           await _mediator.Send(new DeleteNoteCommand(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), noteId));
    }
}
