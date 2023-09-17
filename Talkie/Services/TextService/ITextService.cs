using Talkie.Controllers;
using Talkie.DTOs.Message;

namespace Talkie.Services.TextService
{
    public interface ITextService
    {
        Task<ServiceResponse<GetMessageDto>> SaveText(AddMessageDto message);
    }
}