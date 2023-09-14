using Microsoft.AspNetCore.Mvc;
using Talkie.DTOs.Account;
using Talkie.DTOs.Message;
using Talkie.Services.MessageService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Talkie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<string>>>> GetContactsHistory()
        {
            return Ok(await _messageService.GetMessageHistory());
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<ServiceResponse<List<GetStatementDto>>>> GetAllTransactions()
        {
            return Ok(await _messageService.GetAllTransactions());
        }

        // This route retrieves all messages & transactions i.e. a conversation sent between a User and his friend
        [HttpGet("{contact}")]
        public async Task<ActionResult<ServiceResponse<List<GetMessageDto>>>> GetConversation(string contact)
        {
            return Ok(await _messageService.GetConversation(contact));
        }

        [HttpGet("{contact}/sent")]
        public async Task<ActionResult<ServiceResponse<List<GetMessageDto>>>> GetSentMessages(string contact)
        {
            return Ok(await _messageService.GetSentMessages(contact));
        }

        [HttpGet("{contact}/Received")]
        public async Task<ActionResult<ServiceResponse<List<GetMessageDto>>>> GetReceivedMessages(string contact)
        {
            return Ok(await _messageService.GetReceivedMessages(contact));
        }

        // Send a new message to a friend
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetMessageDto>>> AddMessage(AddMessageDto message)
        {
            return Ok(await _messageService.AddMessage(message));
        }

        [HttpGet("{contact}/Transactions")]
        public async Task<ActionResult<ServiceResponse<List<GetMessageDto>>>> GetTransactions(string contact)
        {
            return Ok(await _messageService.GetTransactions(contact));
        }

        [HttpGet("all")]
        public async Task<ActionResult<ServiceResponse<List<GetAccountDto>>>> GetMessageHistory()
        {
            return Ok(await _messageService.GetContactsHistory());
        }
    }
}