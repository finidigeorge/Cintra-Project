using DataModels;

namespace Repositories.Interfaces
{
    public interface IAuthRepository
    {
        string GeneratePassword(string plainPassword, string salt = null);
        bool IsPasswordValid(User user, string password);
    }
}