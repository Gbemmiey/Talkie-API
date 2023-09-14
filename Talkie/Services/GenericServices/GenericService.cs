﻿using Microsoft.EntityFrameworkCore;
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

        public string GetUserID()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}