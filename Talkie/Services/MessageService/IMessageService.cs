using Talkie.Controllers;
using Talkie.DTOs.Account;
using Talkie.DTOs.Message;

namespace Talkie.Services.MessageService
{
    public interface IMessageService
    {
        Task<ServiceResponse<List<GetMessageDto>>> GetConversation(string contact);

        Task<ServiceResponse<List<GetMessageDto>>> GetSentMessages(string contact);

        Task<ServiceResponse<List<GetMessageDto>>> GetReceivedMessages(string contact);

        Task<ServiceResponse<GetMessageDto>> AddMessage(AddMessageDto newMessage);

        Task<ServiceResponse<List<GetMessageDto>>> GetTransactions(string contact);

        Task<ServiceResponse<List<string>>> GetMessageHistory();

        Task<ServiceResponse<List<GetAccountDto>>> GetContactsHistory();

        Task<ServiceResponse<List<GetStatementDto>>> GetAllTransactions();
    }
}