using MdNotesServer.Core.Stores;
using MediatR;

namespace MdNotes.WebApi.Features.User.Commands
{
    public record CreateNoteCommand(Guid UserId, NoteCore Note) : IRequest<Guid>;

    public class CreateNoteHandler : IRequestHandler<CreateNoteCommand, Guid>
    {
        private readonly INotesStore<NoteCore> _notesStore;

        public CreateNoteHandler(INotesStore<NoteCore> notesStore)
        {
            _notesStore = notesStore;
        }

        public async Task<Guid> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            return await _notesStore.CreateUserNote(request.UserId, request.Note);
        }
    }
}
