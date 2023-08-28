using AutoMapper;
using MdNotesServer.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace MdNotesServer.Infrastructure.Stores
{
    public class NotesStore<T>
    {
        private readonly UsersContext _usersContext;

        private readonly IMapper _mapper;

        public NotesStore(UsersContext context, IMapper mapper)
        {
            _usersContext = context;

            _mapper = mapper;
        }

        public async Task<Guid> CreateUserNote(Guid userId, NoteCore note)
        {
            var user = await _usersContext.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if(user == null)
            {
                var noteEntity = _mapper.Map<NoteEntity>(note);

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

        public async Task<NoteCore> UpdateUserNote(Guid noteId, NoteCore note)
        {
            var noteEntity = await _usersContext.Notes
                .Where(n => n.Id == n.Id)
                .FirstOrDefaultAsync();

            if (note is not null)
            {
                _usersContext.Notes.Update(_mapper.Map<NoteEntity>(note));

                _usersContext.Entry(noteEntity).State = EntityState.Modified;

                await _usersContext.SaveChangesAsync();

                return _mapper.Map<NoteCore>(await _usersContext.Notes.FindAsync(new object[]{ noteId }));
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
