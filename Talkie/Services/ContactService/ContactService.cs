using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talkie.Controllers;
using Talkie.Data;
using Talkie.DTOs.Contact;
using Talkie.Models;
using Talkie.Services.GenericServices;

namespace Talkie.Services.ContactService
{
    public class ContactService : IContactService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IGenericService _genericService;

        public ContactService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, IGenericService genericService)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _genericService = genericService;
        }

        private async Task<bool> UserExists(string accountNumber)
        {
            if (await _context.Accounts.AnyAsync(u => u.Number == accountNumber))
            {
                return true;
            }
            return false;
        }

        public async Task<ServiceResponse<List<GetContactDto>>> AddContact(string accountNumber)
        {
            var serviceResponse = new ServiceResponse<List<GetContactDto>>();

            if (await UserExists(accountNumber))
            {
                Contact newContact = new Contact
                {
                    UserNumber = _genericService.GetUserID(),
                    BeneficiaryNumber = accountNumber
                };

                _context.Contacts.Add(newContact);
                await _context.SaveChangesAsync();
            }
            else
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Account Number does not exist";
            }

            return await GetAllContacts();
        }

        public async Task<ServiceResponse<List<GetContactDto>>> DeleteContact(string accountNumber)
        {
            try
            {
                Contact deletedContact = await _context.Contacts
                                                .Where(c => c.UserNumber == _genericService.GetUserID() && c.BeneficiaryNumber == accountNumber)
                                                .FirstOrDefaultAsync();

                _context.Contacts.Remove(deletedContact);

                await _context.SaveChangesAsync();
                return await GetAllContacts();
            }
            catch (Exception ex)
            {
                var res = await GetAllContacts();
                res.Success = false;
                res.Message = ex.Message;
                return res;
            }
        }

        public async Task<ServiceResponse<List<GetContactDto>>> GetAllContacts()
        {
            var serviceResponse = new ServiceResponse<List<GetContactDto>>();

            List<Contact> contacts = await _context.Contacts
                                    .Where(c => c.UserNumber == _genericService.GetUserID())
                                    .ToListAsync();

            var response = _mapper.Map<List<GetContactDto>>(contacts);
            foreach (GetContactDto item in response)
            {
                var accountDetails = await _context.Accounts.Where(c => c.Number == item.BeneficiaryNumber).FirstOrDefaultAsync();
                item.AccountName = accountDetails.FirstName + " " + accountDetails.LastName;
                item.ProfilePicture = accountDetails.ProfilePictureUrl;
            }

            serviceResponse.Data = response;

            return serviceResponse;
        }
    }
}