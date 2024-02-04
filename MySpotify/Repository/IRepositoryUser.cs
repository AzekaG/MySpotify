using MySpotify.Models;

namespace MySpotify.Repository
{
    public interface IRepositoryUser
    {
         Task<List<User>> GetUserList();
         Task CreateUser(User user);
         Task SaveDb();
         Task DeleteUser(int id);
         void UpdateUser(User user);
        

    }
}
