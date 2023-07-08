using Talkie.DTOs.Account;
using Talkie.Controllers;

public interface IAccountService
{
    Task<ServiceResponse<List<GetAccountDto>>> GetAllAccounts();

    Task<ServiceResponse<GetAccountDto>> GetAccount(string AccountNumber);

    Task<ServiceResponse<GetAccountDto>> ModifyAccount(UpdateAccountDto updatedAccount);

    Task<ServiceResponse<List<GetAccountDto>>> CreateAccount(AddAccountDto newAccount);

    Task<ServiceResponse<GetProfileDto>> GetProfile();
}