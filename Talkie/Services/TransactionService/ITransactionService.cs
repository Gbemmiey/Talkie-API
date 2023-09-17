using Talkie.Controllers;
using Talkie.DTOs.Message;

namespace Talkie.Services.TransactionService
{
    public interface ITransactionService
    {
        Task<ServiceResponse<GetMessageDto>> SendMoney(AddMessageDto message);
    }
}