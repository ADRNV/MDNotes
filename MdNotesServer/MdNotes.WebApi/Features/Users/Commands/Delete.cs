using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MdNotes.WebApi.Features.Users.Commands
{
    public record Delete(Guid Guid) : IRequest<bool>;

    public class DeleteHandler : IRequestHandler<Delete, bool>
    {
        private readonly UserManager<UserEntity> _userManager;

        public DeleteHandler(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> Handle(Delete request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Guid.ToString());

            if(user == null) 
            {
               var result = await _userManager.DeleteAsync(user);

                return result.Succeeded;
            }
            else
            {
                throw new InvalidOperationException("User not found");
            }
            
        }
    }

}
