using AutoMapper;
using MdNotesServer.Core.Stores;
using MdNotesServer.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace MdNotesServer.Infrastructure.Stores
{
    public class NotesStore : INotesStore<NoteCore>
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

        public async Task<NoteCore> UpdateUserNote(Guid userId, Guid noteId, NoteCore note)
        {
            var noteEntity = await _usersContext.Notes
                .Include(n => n.User)
                .Where(n => n.Id == n.Id && n.User.Id == userId)
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

        public async Task<bool> DeleteUserNote(Guid userId, Guid noteId)
        {
            
            var note = await _usersContext.Notes.FindAsync(new object[] {noteId, userId});

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

        public async Task<IEnumerable<NoteCore>> GetUserNotes(Guid userId, int pageSize, int page)
        {
            var user = await _usersContext.Users.FindAsync(new object[] {userId});

            if (user is not null)
            {
                var notes = _usersContext.Notes
                    .Where(n => n.User == user)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .AsEnumerable();

                return _mapper.Map<IEnumerable<NoteCore>>(notes);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
