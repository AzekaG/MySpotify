using Microsoft.EntityFrameworkCore;
using MySpotify.Models;

namespace MySpotify.Repository
{
    public class UserRepository : IRepositoryUser
    {
        readonly MediaUserContext _mediauserContext;
        public UserRepository(MediaUserContext mediaUserContext) => _mediauserContext = mediaUserContext;

        public async Task<List<User>> GetUserList() => await _mediauserContext.Users.Include(x=>x.MediaFiles).ToListAsync();

        public async Task CreateUser(User user) => await _mediauserContext.AddAsync(user);

        public async Task SaveDb() => await _mediauserContext.SaveChangesAsync();

        public async Task DeleteUser(int id)
        {
            var user = await _mediauserContext.Users.FindAsync(id);
            if (user != null)
                _mediauserContext.Users.Remove(user);
        }
        public void UpdateUser(User user) => _mediauserContext.Entry(user).State = EntityState.Modified;



    }
}
