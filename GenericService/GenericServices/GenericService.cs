using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GenericService
{
    public class GenericService : IGenericService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericService _genericService;

        public GenericService(DataContext context, IHttpContextAccessor httpContextAccessor, IGenericService genericService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _genericService = genericService;
        }

        public async Task<decimal> GetUserBalance()
        {
            decimal balance;
            var account = await _context.Accounts.FirstAsync(c => c.Number == _genericService.GetUserID());
            balance = account.Balance;

            return balance;
        }

        public string GetUserID()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}