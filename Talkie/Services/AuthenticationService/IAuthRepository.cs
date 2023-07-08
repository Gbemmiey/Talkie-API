using Talkie.DTOs.Account;
using Talkie.Controllers;

namespace Talkie.Services.Auth
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<string>> Register(AddAccountDto NewAccount);

        Task<ServiceResponse<string>> Login(string Account, string password);

        Task<bool> UserExists(string username);
    }
}