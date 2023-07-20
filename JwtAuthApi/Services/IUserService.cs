using JwtAuthApi.Models;

namespace JwtAuthApi.Services
{
    public interface IUserService
    {
        public void AddUser(User user);
        public User? GetUserByEmail(string email);
        public void UpdateUser(User user);
    }
}

