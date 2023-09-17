using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talkie.Data;

namespace Talkie.Services.GenericServices
{
    public class GenericService : IGenericService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GenericService(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<decimal> GetUserBalance()
        {
            decimal balance;
            var account = await _context.Accounts.FirstAsync(c => c.Number == GetUserID());
            balance = account.Balance;

            return balance;
        }

        public string GetUserID() => _httpContextAccessor.HttpContext
         .User.FindFirstValue(ClaimTypes.NameIdentifier);

        public DateTime getLocalTime()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo wcaZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            // Convert the UTC time to West Central Africa time
            DateTime wcaTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, wcaZone);
            return wcaTime;
        }
    }
}