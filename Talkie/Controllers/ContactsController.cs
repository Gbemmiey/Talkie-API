using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talkie.DTOs.Contact;
using Talkie.Services.ContactService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Talkie.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet("All")]
        public async Task<ActionResult<ServiceResponse<List<GetContactDto>>>> GetAllContacts()
        {
            return Ok(await _contactService.GetAllContacts());
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetContactDto>>>> AddContact(string accountNumber)
        {
            return Ok(await _contactService.AddContact(accountNumber));
        }

        [HttpDelete]
        public async Task<ActionResult<ServiceResponse<List<GetContactDto>>>> DeleteContact(string accountNumber)
        {
            return Ok(await _contactService.DeleteContact(accountNumber));
        }
    }
}