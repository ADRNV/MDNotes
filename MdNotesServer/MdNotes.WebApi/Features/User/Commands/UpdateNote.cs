using MdNotesServer.Core.Stores;
using MediatR;

namespace MdNotes.WebApi.Features.User.Commands
{
    public record UpdateNoteCommand(Guid UserId, Guid NoteId, NoteCore Note) : IRequest<NoteCore>;

    public class UpdateNoteHandler : IRequestHandler<UpdateNoteCommand, NoteCore>
    {
        private readonly INotesStore<NoteCore> _notesStore;

        public UpdateNoteHandler(INotesStore<NoteCore> notesStore)
        {
            _notesStore = notesStore;
        }

        public async Task<NoteCore> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
        {
           return await _notesStore.UpdateUserNote(request.UserId, request.NoteId, request.Note);
        }
    }
}
