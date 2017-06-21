using Shared.Dto;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IAuthController
    {
        Task<JwtTokenDto> Login(UserDto applicationUser);
        string GetPassword(string password);
    }
}
