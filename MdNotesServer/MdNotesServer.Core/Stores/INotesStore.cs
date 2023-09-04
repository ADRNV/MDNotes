using MdNotesServer.Core.Models;

namespace MdNotesServer.Core.Stores
{
    public interface INotesStore<T>
    {
        Task<Guid> CreateUserNote(Guid userId, T note);

        Task<T> UpdateUserNote(Guid userId, Guid noteId, T note);

        Task<bool> DeleteUserNote(Guid userId, Guid noteId);

        Task<IEnumerable<T>> GetUserNotes(Guid userId, int pageSize, int page);
    }
}
