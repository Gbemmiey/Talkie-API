using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talkie.Controllers;
using Talkie.Data;
using Talkie.DTOs.Account;
using Talkie.Models;

namespace Talkie.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserId() => _httpContextAccessor.HttpContext
           .User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<ServiceResponse<List<GetAccountDto>>> CreateAccount(AddAccountDto newAccount)
        {
            var serviceResponse = new ServiceResponse<List<GetAccountDto>>();
            Account dbAccount = _mapper.Map<Account>(newAccount);

            _context.Accounts.Add(dbAccount);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Accounts
                    .Select(c => _mapper.Map<GetAccountDto>(c))
                    .ToListAsync();

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetAccountDto>>> GetAllAccounts()
        {
            var service_response = new ServiceResponse<List<GetAccountDto>>();
            var dbAccounts = await _context.Accounts.ToListAsync();

            service_response.Data = dbAccounts.Select(c => _mapper.Map<GetAccountDto>(c)).ToList();
            return service_response;
        }

        public async Task<ServiceResponse<GetAccountDto>> GetAccount(string accountNumber)
        {
            var service_response = new ServiceResponse<GetAccountDto>();
            var dbAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.Number == accountNumber);

            service_response.Data = _mapper.Map<GetAccountDto>(dbAccount);
            return service_response;
        }

        public async Task<ServiceResponse<GetAccountDto>> ModifyAccount(UpdateAccountDto updatedAccount)
        {
            var service_response = new ServiceResponse<GetAccountDto>();

            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(c => c.Number == GetUserId());
                _mapper.Map(account, updatedAccount);

                await _context.SaveChangesAsync();

                service_response.Data = _mapper.Map<GetAccountDto>(account);
            }
            catch (Exception ex)
            {
                service_response.Success = false;
                service_response.Message = ex.Message;
            }
            return service_response;
        }

        public async Task<ServiceResponse<GetProfileDto>> GetProfile()
        {
            var service_response = new ServiceResponse<GetProfileDto>();
            var dbAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.Number == GetUserId());

            service_response.Data = _mapper.Map<GetProfileDto>(dbAccount);
            return service_response;
        }
    }
}