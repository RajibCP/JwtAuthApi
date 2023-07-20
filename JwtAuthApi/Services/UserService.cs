using JwtAuthApi.Models;

namespace JwtAuthApi.Services
{
    public class UserService : IUserService
    {
        private List<User> users = new();

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public User? GetUserByEmail(string email)
        {
            return users.FirstOrDefault(x => x.Email == email);
        }

        public void UpdateUser(User user)
        {
            User? targetUser = users.FirstOrDefault(x => x.Email == user.Email);
            if(targetUser != null)
            {
                users[users.IndexOf(targetUser)] = user;
            }
        }
    }
}

