using MdNotesServer.Core.Stores;
using MediatR;

namespace MdNotes.WebApi.Features.User.Commands
{
    public record DeleteNoteCommand(Guid UserId, Guid NoteId) : IRequest<bool>;

    public class DeleteNoteHandler : IRequestHandler<DeleteNoteCommand, bool>
    {
        private readonly INotesStore<NoteCore> _notesStore;

        public DeleteNoteHandler(INotesStore<NoteCore> notesStore)
        {
            _notesStore = notesStore;
        }

        public async Task<bool> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            return await _notesStore.DeleteUserNote(request.UserId, request.NoteId);
        }
    }
}
