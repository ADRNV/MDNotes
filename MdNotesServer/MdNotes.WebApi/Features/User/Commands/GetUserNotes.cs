using MdNotesServer.Core.Stores;
using MediatR;

namespace MdNotes.WebApi.Features.User.Commands
{
    public record GetUserNotesCommand(Guid UserId, int PageSize, int Page) : IRequest<IEnumerable<NoteCore>>;

    public class GetUserNotesHandler : IRequestHandler<GetUserNotesCommand, IEnumerable<NoteCore>>
    {
        private INotesStore<NoteCore> _store;

        public GetUserNotesHandler(INotesStore<NoteCore> notesStore)
        {
            _store = notesStore;
        }
        public async Task<IEnumerable<NoteCore>> Handle(GetUserNotesCommand request, CancellationToken cancellationToken)
        {
           return await _store.GetUserNotes(request.UserId, request.PageSize, request.Page);
        }
    }


}
