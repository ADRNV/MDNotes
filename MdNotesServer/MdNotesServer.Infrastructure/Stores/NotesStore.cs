using Microsoft.EntityFrameworkCore;

namespace MdNotesServer.Infrastructure.Stores
{
    public class NotesStore<T>
    {
        private readonly UsersContext _usersContext;

        public NotesStore(UsersContext context)
        {
            _usersContext = context;
        }

        public async Task<Guid> CreateUserNote(Guid userId, NoteEntity noteEntity)
        {
            var user = await _usersContext.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if(user == null)
            {
                user.Notes.Add(noteEntity);

                _usersContext.Entry(user).State = EntityState.Modified;

                await _usersContext.SaveChangesAsync();

                return userId;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<NoteEntity> UpdateUserNote(Guid noteId, NoteEntity noteEntity)
        {
            var note = await _usersContext.Notes
                .Where(n => n.Id == n.Id)
                .FirstOrDefaultAsync();

            if (note is not null)
            {
                _usersContext.Notes.Update(noteEntity);

                _usersContext.Entry(noteEntity).State = EntityState.Modified;

                await _usersContext.SaveChangesAsync();

                return await _usersContext.Notes.FindAsync(new object[]{ noteId });
            }
            else
            {
                throw new InvalidOperationException();
            }
            
        }

        public async Task<bool> DeleteUserNote(Guid noteId)
        {
            
            var note = await _usersContext.Notes.FindAsync(new object[] {noteId});

            if(note is not null)
            {
                _usersContext.Notes.Remove(note);
            }
            else
            {
                throw new InvalidOperationException();
            }

            return note is null;
            
        }
    }
}
