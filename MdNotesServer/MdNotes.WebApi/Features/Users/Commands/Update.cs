using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MdNotes.WebApi.Features.Users.Commands
{
    public record Update(Guid Guid, UserCore User) : IRequest<bool>;

    public class UpdateHandler : IRequestHandler<Update, bool>
    {
        private readonly UserManager<UserEntity> _userManager;

        private readonly IMapper _mapper;

        public UpdateHandler(UserManager<UserEntity> userManager, IMapper mapper)
        {
            _userManager = userManager;

            _mapper = mapper;
        }

        public async Task<bool> Handle(Update request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Guid.ToString());

            var newUser = request.User;

            if (user is not null)
            {
                var result = await _userManager.UpdateAsync(_mapper.Map<UserCore, UserEntity>(newUser));

                return result.Succeeded;
            }
            else
            {
                throw new InvalidOperationException("User not found");
            }
            
        }
    }
}
