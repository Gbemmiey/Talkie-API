using Talkie.Controllers;
using Talkie.DTOs.Contact;

namespace Talkie.Services.ContactService
{
    public interface IContactService
    {
        Task<ServiceResponse<List<GetContactDto>>> GetAllContacts();

        Task<ServiceResponse<List<GetContactDto>>> AddContact(string accountNumber);

        Task<ServiceResponse<List<GetContactDto>>> DeleteContact(string accountNumber);
    }
}