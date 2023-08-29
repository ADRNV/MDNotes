using MdNotesServer.Core.Models;

namespace MdNotesServer.Core.Stores
{
    public interface INotesStore<T>
    {
        Task<Guid> CreateUserNote(Guid userId, Note note);

        Task<T> UpdateUserNote(Guid noteId, T note);

        Task<bool> DeleteUserNote(Guid noteId);

        Task<IEnumerable<T>> GetUserNotes(Guid userId, int pageSize, int page);
    }
}
